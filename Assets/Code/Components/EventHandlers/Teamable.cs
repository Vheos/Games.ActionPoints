namespace Vheos.Games.ActionPoints
{
    using UnityEngine;
    using Tools.UnityCore;

    public class Teamable : ABaseComponent
    {
        // Inspector
        [SerializeField] protected Team.Predefined _StartingTeam;

        // Events
        public Event<Team, Team> OnTeamChanged
        { get; } = new Event<Team, Team>();

        // Publics
        public Team Team
        { get; private set; }
        public void ChangeTeam(Team newTeam)
        {
            if (newTeam == Team)
                return;

            Team previousTeam = Team;
            if (Team != null)
            {
                Team.RemoveMember(this);
                Team = null;
            }
            if (newTeam != null)
            {
                newTeam.AddMember(this);
                Team = newTeam;
            }
            OnTeamChanged?.Invoke(previousTeam, Team);
        }
        public void LeaveTeam()
        => ChangeTeam(null);

        // Privates
        private Team StartingTeam
        => _StartingTeam switch
        {
            Team.Predefined.Players => Team.Players,
            Team.Predefined.Enemies => Team.Enemies,
            _ => null,
        };

        // Play
        protected override void PlayStart()
        {
            base.PlayStart();
            ChangeTeam(StartingTeam);
        }
    }
}