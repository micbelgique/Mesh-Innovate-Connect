using System.Numerics;
using Microsoft.Mesh.CloudScripting;
using Newtonsoft.Json;

namespace Presentation1
{
    public class QLearning
    {
        private double[,] qTable; // Table for the memory of the npc, works with a reward value
        private double[] maxQValues; // Array that stores the maximum Q value for each state
        private int[] maxQActions; // Array that the action with the max Q value
        private int numStates, numActions; //numState : number of stats possible | numActions : number of actions posible
        private double learningRate, discountFactor, explorationRate; // Parameters for the Q learning algorithm
        private float lastDistance = 9999; // Stores the last distance from the choosen destination
        private Random rnd = new Random();
        const int GRID_SIZE = 60;
        public QLearning(int numStates, int numActions, double learningRate, double discountFactor, double explorationRate)
        {
            this.numStates = numStates;
            this.numActions = numActions;
            this.learningRate = learningRate;
            this.discountFactor = discountFactor;
            this.explorationRate = explorationRate;

            qTable = new double[numStates, numActions];
            maxQValues = new double[numStates];
            maxQActions = new int[numStates];
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
                return maxQActions[state];
            }
        }

        // Update the Q value in the Q table with the reward it gets
        public void UpdateQValue(int prevState, int action, float reward, int nextState)
        {
            // Bellman's Equation
            qTable[prevState, action] += learningRate * (reward + discountFactor * maxQValues[nextState] - qTable[prevState, action]);
            if (qTable[prevState, action] > maxQValues[prevState])
            {
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
            int state = Math.Abs(z * 100 + x) % 100000;

            return state;
        }

        // Main function that make the npc move and calls all the subfunctions
        public async void MoveAction(TransformNode npc, Vector3 destination, int npcNum, int numIterations)
        {
            float duration = 2f;
            float stepSize = 0.01f / duration;
            LoadQTable(npcNum);

            bool[,] gridObstacles = GridObstacle();


            for (int i = 0; i < numIterations; i++)
            {
                int prevState = GetState(npc);
                int action = ChooseAction(prevState);

                Vector3 direction = Vector3.Zero;

                // Move the NPC based on the action
                switch (action)
                {
                    case 0: direction = new Vector3(0, 0, 2); break; // Move up
                    case 1: direction = new Vector3(0, 0, -2); break; // Move down
                    case 2: direction = new Vector3(-2, 0, 0); break; // Move left  
                    case 3: direction = new Vector3(2, 0, 0); break; // Move right
                    case 4: direction = new Vector3(1, 0f, 1); break; // Move up-right
                    case 5: direction = new Vector3(-1, 0f, 1); break; // Move up-left
                    case 6: direction = new Vector3(1, 0f, -1); break; // Move down-right
                    case 7: direction = new Vector3(-1, 0f, -1); break; // Move down-left
                }

                Vector3 desiredPosition = npc.Position + direction;
                if (desiredPosition.X >= -GRID_SIZE / 2 && desiredPosition.X < GRID_SIZE / 2 && desiredPosition.Z >= -GRID_SIZE / 2 && desiredPosition.Z < GRID_SIZE / 2 && !gridObstacles[(int)desiredPosition.X + GRID_SIZE/2, (int)desiredPosition.Z+ GRID_SIZE/2])
                {
                        float t = 0f;
                    while (t < 1f)
                    {
                        npc.Position = Vector3.Lerp(npc.Position, desiredPosition, t);
                        t += stepSize;
                        if (t > 1f)
                            t = 1f;
                        //await Task.Delay(1);
                    }
                }

                // Calculate the reward
                float reward = CalculateReward(npc, destination);

                int nextState = GetState(npc);

                // Update the Q-value
                UpdateQValue(prevState, action, reward, nextState);
                SaveQTable(npcNum);
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

        public bool[,] GridObstacle()
        {
            bool[,] grid = new bool[GRID_SIZE, GRID_SIZE];
            for (int x = 0; x < GRID_SIZE; x++)
            {
                for (int z = 0; z < GRID_SIZE; z++)
                {
                    grid[x, z] = false;
                }
            }

            // Add walls

            // Add walls
            for (int z = -10; z <= 10; z++)
            {
                grid[0 + GRID_SIZE / 2, z + GRID_SIZE / 2] = true; 
                grid[1 + GRID_SIZE / 2, z + GRID_SIZE / 2] = true; 
                grid[-29 + GRID_SIZE / 2, z + GRID_SIZE / 2] = true; 
                grid[-30 + GRID_SIZE / 2, z + GRID_SIZE / 2] = true; 
            }

            for (int x = -30; x < 0; x++)
            {
                grid[x + GRID_SIZE / 2, 10 + GRID_SIZE / 2] = true; 
                grid[x + GRID_SIZE / 2, 11 + GRID_SIZE / 2] = true; 
                grid[x + GRID_SIZE / 2, -10 + GRID_SIZE / 2] = true; 
                grid[x + GRID_SIZE / 2, -11 + GRID_SIZE / 2] = true; 
            }

            return grid;
        }
    }
}

