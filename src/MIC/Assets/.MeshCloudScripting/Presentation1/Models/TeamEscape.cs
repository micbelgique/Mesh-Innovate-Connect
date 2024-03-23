using Microsoft.Mesh.CloudScripting;
using System.Diagnostics.Eventing.Reader;


namespace Presentation1.Models
{
    public class TeamEscape
    {
        public string Name { get; }
        public List<Avatar> Participants { get; }

        public TeamEscape(string name)
        {
            Name = name;
            Participants = new List<Avatar>();
        }

        public void AddParticipant(Avatar avatar)
        {
            Participants.Add(avatar);
        }

        public void RemoveParticipant(Avatar avatar)
        {
            Participants.Remove(avatar);
        }


        public static void InitTeams(TeamEscape redTeam, TeamEscape greenteam, List<Avatar> participants)
        {
            Random random = new Random();
            List<Avatar> shuffledParticipants = participants.OrderBy(x => random.Next()).ToList();
            bool addRedTeam = false;

            foreach (var participant in shuffledParticipants)
            {
                if (addRedTeam)
                {
                    greenteam.AddParticipant(participant);
                    addRedTeam = false;
                }
                else
                {
                    redTeam.AddParticipant(participant);
                    addRedTeam = true;
                }

            }
        }

    }
}
