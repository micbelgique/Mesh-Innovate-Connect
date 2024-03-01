using Microsoft.Mesh.CloudScripting;

namespace Presentation1
{
    public class PlayerEscape
    {
        public Avatar Avatar;
        public string Team;
        public PlayerEscape(Avatar avatar)
        {
            Avatar = avatar;
        }


        public static void SetEquipe(PlayerEscape player, List<PlayerEscape> players)
        {
            if (players.Count % 2 == 0) player.Team = "Green Team";
            else player.Team = "Red Team";
        }
        


    }


}
