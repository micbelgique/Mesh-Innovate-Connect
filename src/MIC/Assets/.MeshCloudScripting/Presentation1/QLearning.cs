using System.Numerics;
using Microsoft.Mesh.CloudScripting;

namespace Presentation1
{
    public class QLearning
    {
        private double[,] qTable;
        private int numStates, numActions;
        private double learningRate, discountFactor, explorationRate;
        private float lastDistance = 9999;
        public QLearning(int numStates, int numActions, double learningRate, double discountFactor, double explorationRate)
        {
            this.numStates = numStates;
            this.numActions = numActions;
            this.learningRate = learningRate;
            this.discountFactor = discountFactor;
            this.explorationRate = explorationRate;

            qTable = new double[numStates, numActions];
        }

        public int ChooseAction(int state)
        {
            var rnd = new Random();
            if (rnd.NextDouble() < explorationRate)
            {
                // Explore: select a random action
                return rnd.Next(numActions);
            }
            else
            {
                // Exploit: select the action with max value (greedy)
                var maxVal = double.MinValue;
                var maxAction = 0;
                for (var action = 0; action < numActions; action++)
                {
                    if (qTable[state, action] > maxVal)
                    {
                        maxVal = qTable[state, action];
                        maxAction = action;
                    }
                }
                return maxAction;
            }
        }

        public void UpdateQValue(int prevState, int action, int reward, int nextState)
        {
            var maxQ = double.MinValue;
            for (var i = 0; i < numActions; i++)
            {
                if (qTable[nextState, i] > maxQ)
                {
                    maxQ = qTable[nextState, i];
                }
            }

            qTable[prevState, action] += learningRate * (reward + discountFactor * maxQ - qTable[prevState, action]);
        }
        public int GetState(TransformNode npc)
        {
            // Assuming your grid is 10x10 and each cell is 1 unit
            int x = (int)Math.Round(npc.Position.X);
            int z = (int)Math.Round(npc.Position.Z);

            // Convert the 2D position to a single index
            int state = Math.Abs(z * 10 + x);
            while (state >= 100)
            {
                state = state/10;
            }

            return state;
        }

        public async void MoveAction(TransformNode npc, int numIterations)
        {
            for (int i = 0; i < numIterations; i++)
            {
                int prevState = GetState(npc);
                int action = ChooseAction(prevState);

                // Move the NPC based on the action
                switch (action)
                {
                    case 0:  // Move up
                        npc.Position = Vector3.Lerp(npc.Position, npc.Position + new Vector3(0, 0, 1), 0.6f);
                        await Task.Delay(100);
                        break;
                    case 1:  // Move down
                        npc.Position = Vector3.Lerp(npc.Position, npc.Position + new Vector3(0, 0, -1), 0.6f);
                        await Task.Delay(100);
                        break;
                    case 2:  // Move left
                        npc.Position = Vector3.Lerp(npc.Position, npc.Position + new Vector3(-1, 0, 0), 0.6f);
                        await Task.Delay(100);
                        break;
                    case 3:  // Move right
                        npc.Position = Vector3.Lerp(npc.Position, npc.Position + new Vector3(1, 0, 0), 0.6f);
                        await Task.Delay(100);
                        break;
                }

                // Calculate the reward
                int reward = CalculateReward(npc);

                int nextState = GetState(npc);

                // Update the Q-value
                UpdateQValue(prevState, action, reward, nextState);
            }
        }
        public int CalculateReward(TransformNode npc)
        {
            Vector3 goalPosition = new Vector3(6, 0, -10);
            float distance = Vector3.Distance(npc.Position, goalPosition);

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
    }
}
