using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using Microsoft.Mesh.CloudScripting;
using Newtonsoft.Json;

namespace Presentation1
{
    public class QLearning
    {
        private double[,] qTable; // Table for the memory of the npc, works with a reward value
        private Dictionary<int, double> maxQValues = new Dictionary<int, double>();
        private Dictionary<int, int> maxQActions = new Dictionary<int, int>();
        private int numStates, numActions; //numState : number of stats possible | numActions : number of actions posible
        private double learningRate, discountFactor, explorationRate; // Parameters for the Q learning algorithm
        private float lastDistance = 9999; // Stores the last distance from the choosen destination
        private Random rnd = new Random();
        const int GRID_SIZE = 60;
        const int STATE_MODULUS = 100000;
        private Dictionary<int, Vector3> actionDirections = new Dictionary<int, Vector3>
        {
            {0, new Vector3(0, 0, 1)}, // Move up
            {1, new Vector3(0, 0, -1)}, // Move down
            {2, new Vector3(-1, 0, 0)}, // Move left  
            {3, new Vector3(1, 0, 0)}, // Move right
            {4, new Vector3(1, 0f, 1)}, // Move up-right
            {5, new Vector3(-1, 0f, 1)}, // Move up-left
            {6, new Vector3(1, 0f, -1)}, // Move down-right
            {7, new Vector3(-1, 0f, -1)} // Move down-left
        };
        private bool[,] gridObstacles;
        public QLearning(int numStates, int numActions, double learningRate, double discountFactor, double explorationRate, int npcNum)
        {
            this.numStates = numStates;
            this.numActions = numActions;
            this.learningRate = learningRate;
            this.discountFactor = discountFactor;
            this.explorationRate = explorationRate;

            qTable = new double[numStates, numActions];

            LoadQTable(npcNum);
            gridObstacles = LoadGrid();
        }

        // Class to choose the next action that the npc will do
        public int ChooseAction(int state)
        {
            // Change the explorationRate to have a more explorating or a more exploiting npc
            if (rnd.NextDouble() < explorationRate)
            {
                // Explore: select a random action
                return rnd.Next(numActions);
            }
            else
            {
                // Exploit: select the action with max value
                return maxQActions.ContainsKey(state) ? maxQActions[state] : 0;
            }
        }

        // Update the Q value in the Q table with the reward it gets
        public void UpdateQValue(int prevState, int action, float reward, int nextState)
        {
            double oldValue = qTable[prevState, action];
            if (!maxQValues.ContainsKey(prevState) || qTable[prevState, action] > maxQValues[prevState])
            {
                double learnedValue = reward + discountFactor * (maxQValues.ContainsKey(nextState) ? maxQValues[nextState] : 0);
                qTable[prevState, action] += learningRate * (learnedValue - oldValue);
                maxQValues[prevState] = qTable[prevState, action];
                maxQActions[prevState] = action;
            }
        }

        // Get the position/state of the npc
        public int GetState(TransformNode npc)
        {
            int x = (int)Math.Round(npc.Position.X);
            int z = (int)Math.Round(npc.Position.Z);

            // Convert the 2D position to a single index
            int state = Math.Abs(z * 100 + x) % STATE_MODULUS;

            return state;
        }

        // Main function that make the npc move and calls all the subfunctions
        public async void MoveAction(TransformNode npc, Vector3 destination, int npcNum, int numIterations)
        {
            for (int i = 0; i < numIterations; i++)
            {
                int prevState = GetState(npc);
                int action = ChooseAction(prevState);

                Vector3 direction = actionDirections[action];

                await RotateNpc(npc, direction);
                await MoveNpc(npc, direction, npcNum);

                // Calculate the reward
                float reward = CalculateReward(npc, destination);

                int nextState = GetState(npc);

                // Update the Q-value
                UpdateQValue(prevState, action, reward, nextState);
                if (i % 100 == 0)
                {
                    SaveQTable(npcNum);
                }
            }
        }

        public async Task MoveNpc(TransformNode npc, Vector3 direction, int npcNum)
        {
            float duration = 2f;
            float remainingTime = duration;
            Vector3 desiredPosition = npc.Position + direction;

            if (desiredPosition.X >= -GRID_SIZE / 2 && desiredPosition.X < GRID_SIZE / 2 && desiredPosition.Z >= -GRID_SIZE / 2 && desiredPosition.Z < GRID_SIZE / 2 && !gridObstacles[(int)desiredPosition.X + GRID_SIZE / 2, (int)desiredPosition.Z + GRID_SIZE / 2])
            {
                float t = 0f;
                while (t < 1f)
                {
                    float delatTime = 0.01f;
                    float stepSize = delatTime / duration;
                    t += stepSize;
                    npc.Position = Vector3.Lerp(npc.Position, desiredPosition, t);

                    if (t > 1f) t = 1f;
                    await Task.Delay((int)(delatTime * 1000));
                    remainingTime -= delatTime;
                }
            }

        }

        public async Task RotateNpc(TransformNode npc, Vector3 direction)
        {
            Vector3 normalizedDirection = Vector3.Normalize(direction);
            
            float rotationAngleRadians = MathF.Atan2(normalizedDirection.X, normalizedDirection.Z);
            if (rotationAngleRadians == 0)
            {
                return;
            }

            Vector3 rotationAxis = new Vector3(0, MathF.Sin(rotationAngleRadians / 2), 0);
            rotationAxis = Vector3.Normalize(rotationAxis);
            Quaternion rotation = new Quaternion(rotationAxis.X, rotationAxis.Y, rotationAxis.Z, MathF.Cos(rotationAngleRadians / 2));
            if (Equals(npc.Rotation, rotation)) return;

            float duration = 1f;
            float remainingTime = duration;
            float t = 0f;
            while (t < 1f)
            {
                float deltaTime = 0.01f; // Adjust as needed
                float stepSize = deltaTime / duration;

                t += stepSize;
                npc.Rotation = Quaternion.Slerp(npc.Rotation, rotation, t);

                if (t > 1f) t = 1f;

                await Task.Delay((int)(deltaTime * 1000)); // Convert deltaTime to milliseconds
                remainingTime -= deltaTime;
            }
        }


        // Calculate the reward for the movement with the distance of the final destination
        public int CalculateReward(TransformNode npc, Vector3 destination)
        {
            float distance = Vector3.Distance(npc.Position, destination);

            if (distance == 0)
            {
                return 100;  // Big reward for reaching the goal
            }
            else if (distance >= lastDistance)
            {
                lastDistance = distance;
                return -1;
            }
            {
                lastDistance = distance;
                return 1;
            }
        }

        // At the end of the movement, save the Q table in a json to exploit it at the next launchs 
        public void SaveQTable(int npcNum)
        {
            var qTableList = new List<List<double>>();
            for (int i = 0; i < numStates; i++)
            {
                var row = new List<double>();
                for (int j = 0; j < numActions; j++)
                {
                    row.Add(qTable[i, j]);
                }
                qTableList.Add(row);
            }
            string finalFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "qtable" + npcNum + ".json"); ;
            File.WriteAllText(finalFilePath, JsonConvert.SerializeObject(qTableList));
        }


        // Load the Q table 
        public void LoadQTable(int npcNum)
        {
            string finalFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "qtable" + npcNum + ".json");
            if (File.Exists(finalFilePath))
            {
                var qTableList = JsonConvert.DeserializeObject<List<List<double>>>(File.ReadAllText(finalFilePath));
                for (int i = 0; i < numStates; i++)
                {
                    for (int j = 0; j < numActions; j++)
                    {
                        qTable[i, j] = qTableList[i][j];
                    }
                }
            }
        }

        public bool[,] LoadGrid()
        {
            string finalFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "walls.json");
            if (File.Exists(finalFilePath))
            {
                var gridList = JsonConvert.DeserializeObject<List<List<bool>>>(File.ReadAllText(finalFilePath));
                bool[,] grid = new bool[GRID_SIZE, GRID_SIZE];
                for (int x = 0; x < GRID_SIZE; x++)
                {
                    for (int z = 0; z < GRID_SIZE; z++)
                    {
                        grid[x, z] = gridList[x][z];
                    }
                }
                return grid;
            }
            else
            {
                return new bool[GRID_SIZE, GRID_SIZE];
            }
        }
    }

    internal record struct NewStruct(int Item1, int Item2, int Item3)
    {
        public static implicit operator (int, int, int)(NewStruct value)
        {
            return (value.Item1, value.Item2, value.Item3);
        }

        public static implicit operator NewStruct((int, int, int) value)
        {
            return new NewStruct(value.Item1, value.Item2, value.Item3);
        }
    }
}