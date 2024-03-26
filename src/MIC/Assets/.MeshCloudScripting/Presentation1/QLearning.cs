using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Numerics;
using Microsoft.Mesh.CloudScripting;
using Newtonsoft.Json;

namespace Presentation1
{
    public class QLearning
    {
        const int GRID_SIZE = 60;
        const int STATE_MODULUS = 100000;
        const float DISTANCE_THRESHOLD = 0.1f;
        const int REWARD_GOAL = 100;
        const int REWARD_FAR = -1;
        const int REWARD_CLOSE = 1;

        private readonly double[,] qTable; // Table for the memory of the npc, works with a reward value
        private readonly int numStates, numActions; //numState : number of stats possible | numActions : number of actions posible
        private readonly double learningRate, discountFactor, explorationRate; // Parameters for the Q learning algorithm
        private readonly bool[,] gridObstacles;
        private readonly object lockObject = new object();
        private readonly Random rnd = new Random();

        private List<Vector3> npcPositions = new List<Vector3>();

        private float lastDistance = 9999; // Stores the last distance from the choosen destination

        private readonly Dictionary<int, double> maxQValues = new Dictionary<int, double>();
        private readonly Dictionary<int, int> maxQActions = new Dictionary<int, int>();
        private readonly Dictionary<float, Vector3> actionDirections = new Dictionary<float, Vector3>
        {
            {0, new Vector3(0, 0, 1f)}, // Move up
            {1, new Vector3(0, 0, -1f)}, // Move down
            {2, new Vector3(-1f, 0, 0)}, // Move left  
            {3, new Vector3(1f, 0, 0)}, // Move right
            {4, new Vector3(1f, 0f, 1f)}, // Move up-right
            {5, new Vector3(-01f, 0f, 1f)}, // Move up-left
            {6, new Vector3(1f, 0f, -1f)}, // Move down-right
            {7, new Vector3(-1f, 0f, -1f)} // Move down-left
        };

        public QLearning(int numStates, int numActions, double learningRate, double discountFactor, double explorationRate, int npcNum)
        {
            this.numStates = numStates;
            this.numActions = numActions;
            this.learningRate = learningRate;
            this.discountFactor = discountFactor;
            this.explorationRate = explorationRate;

            qTable = new double[numStates, numActions];

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
                maxQActions.TryGetValue(state, out int action);
                // Exploit: select the action with max value
                return action;
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
        public async void MoveAction(TransformNode npc, Vector3 destination, string destinationName, int npcNum, int numIterations, CancellationToken cancellationToken)
        {
            LoadQTable(npcNum, destinationName);
            for (int i = 0; i < numIterations; i++)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }
                int prevState = GetState(npc);
                int action = ChooseAction(prevState);

                Vector3 direction = actionDirections[action];

                await RotateNpc(npc, direction, cancellationToken);
                await MoveNpc(npc, direction, npcNum, cancellationToken);

                // Calculate the reward
                float reward = CalculateReward(npc, destination);

                int nextState = GetState(npc);

                // Update the Q-value
                UpdateQValue(prevState, action, reward, nextState);
                if (i % 100 == 0)
                {
                    SaveQTable(npcNum, destinationName);
                }
                if (Vector3.Distance(npc.Position, destination) <= DISTANCE_THRESHOLD) // 0.1f is a small threshold to account for floating point precision
                {
                    break; // Stop the movement
                }
            }
        }

        public async Task MoveNpc(TransformNode npc, Vector3 direction, int npcNum, CancellationToken cancellationToken)
        {
            float duration = 2f;
            Vector3 desiredPosition = npc.Position + direction;

            if (npcPositions.Any(pos => Vector3.Distance(pos, desiredPosition) < DISTANCE_THRESHOLD))
            {
                // If there is a collision, return without moving the NPC
                return;
            }

            if (desiredPosition.X >= -GRID_SIZE / 2 && desiredPosition.X < GRID_SIZE / 2 && desiredPosition.Z >= -GRID_SIZE / 2 && desiredPosition.Z < GRID_SIZE / 2 && !gridObstacles[(int)desiredPosition.X + GRID_SIZE / 2, (int)desiredPosition.Z + GRID_SIZE / 2])
            {
                float t = 0f;
                while (t < 1f)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        return;
                    }
                    float deltaTime = 0.02f;
                    float stepSize = deltaTime / duration;
                    t += stepSize;
                    npc.Position = Vector3.Lerp(npc.Position, desiredPosition, t);

                    if (t > 1f) t = 1f;
                    await Task.Delay((int)(deltaTime * 100));
                }
            }

        }

        public async Task RotateNpc(TransformNode npc, Vector3 direction, CancellationToken cancellationToken)
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

            float duration = 0.5f;
            float t = 0f;
            while (t < 1f)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }
                float deltaTime = 0.02f; // Adjust as needed
                float stepSize = deltaTime / duration;

                t += stepSize;
                npc.Rotation = Quaternion.Slerp(npc.Rotation, rotation, t * t * (3 - 2 * t));

                if (t > 1f) t = 1f;

                await Task.Delay((int)(deltaTime * 100));
            }
        }


        // Calculate the reward for the movement with the distance of the final destination
        public int CalculateReward(TransformNode npc, Vector3 destination)
        {
            float distance = Vector3.Distance(npc.Position, destination);

            if (distance == 0)
            {
                return REWARD_GOAL;  // Big reward for reaching the goal
            }
            else
            {
                return -1 * (int)distance;
            }
        }

        // At the end of the movement, save the Q table in a json to exploit it at the next launchs 
        public void SaveQTable(int npcNum, string destinationName)
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
                string finalFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "qtable" + npcNum + destinationName + ".json"); ;
                File.WriteAllText(finalFilePath, JsonConvert.SerializeObject(qTableList));
        }


        // Load the Q table 
        public void LoadQTable(int npcNum, string destinationName)
        {
                string finalFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "qtable" + npcNum + destinationName + ".json");
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