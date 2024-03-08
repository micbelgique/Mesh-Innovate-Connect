using Microsoft.Mesh.CloudScripting;
using System.Numerics;

public class PersonFlow
{
    public async void Boucle(TransformNode npc, int deplacementMax)
    {
        Vector3 positionInitiale = npc.Position;
        float deplacement = 0;
        for (int i = 0; i < 2; i++)
        {
            while (deplacement <= deplacementMax)
            {
                positionInitiale.X = (float)(positionInitiale.X + 0.01);
                npc.Position = positionInitiale;
                deplacement = (float)(deplacement + 0.01);
                await Task.Delay(10);
            }
            while (deplacement >= 0)
            {
                positionInitiale.Z = (float)(positionInitiale.Z + 0.01);
                npc.Position = positionInitiale;
                deplacement = (float)(deplacement - 0.01);
                await Task.Delay(10);
            }
            while (deplacement <= deplacementMax)
            {
                positionInitiale.X = (float)(positionInitiale.X - 0.01);
                npc.Position = positionInitiale;
                deplacement = (float)(deplacement + 0.01);
                await Task.Delay(10);
            }
            while (deplacement >= 0)
            {
                positionInitiale.Z = (float)(positionInitiale.Z - 0.01);
                npc.Position = positionInitiale;
                deplacement = (float)(deplacement - 0.01);
                await Task.Delay(10);
            }
        }
    }
}