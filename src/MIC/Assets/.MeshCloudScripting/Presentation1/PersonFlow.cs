using Microsoft.Mesh.CloudScripting;
using System.Numerics;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;


namespace Presentation1
{
    public class PersonFlow
    {
        public async void Boucle(TransformNode npc, int deplacementMax)
        {
            Vector3 positionInitiale = npc.Position;
            float deplacement = 0;
            for (int i = 0; i < 10; i++)
            {
                while (deplacement <= deplacementMax)
                {
                    Vector3 destinationPosition1 = positionInitiale + new Vector3(4f, 0f, 0f);
                    Vector3 destinationPosition2 = destinationPosition1 + new Vector3(0f, 0f, 4f);
                    Vector3 destinationPosition3 = destinationPosition2 + new Vector3(-4f, 0f, 0f);

                    await MoveToPosition(destinationPosition1, positionInitiale);

                    await MoveToPosition(destinationPosition2, destinationPosition1);

                    await MoveToPosition(destinationPosition3, destinationPosition2);

                    await MoveToPosition(positionInitiale, destinationPosition3);
                }

                async Task MoveToPosition(Vector3 destinationPosition, Vector3 initialPosition)
                {
                    float t = 0f;
                    float duration = 2f;
                    float stepSize = 0.01f / duration;

                    while (t < 1f)
                    {
                        npc.Position = Vector3.Lerp(initialPosition, destinationPosition, t);
                        t += stepSize;
                        if (t > 1f)
                            t = 1f;
                        await Task.Delay(10);
                    }
                }
            }
        }
    }
}