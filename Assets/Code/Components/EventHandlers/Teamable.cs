namespace Vheos.Games.ActionPoints
{
    using UnityEngine;
    using Tools.UnityCore;

    public class Teamable : ABaseComponent
    {
        // Events
        public Event<Team, Team> OnTeamChanged
        { get; } = new Event<Team, Team>();

        // Input
        public ComponentInput<Team> StartingTeam
        { get; } = new ComponentInput<Team>();

        // Publics
        public Team Team
        { get; private set; }
        public bool HasAllies
        => Team != null && Team.Count > 1;
        public bool IsAlliedWith(Teamable other)
        => this != other && Team == other.Team;
        public void TryChangeTeam(Team newTeam)
        {
            if (!enabled
            || newTeam == Team)
                return;

            Team previousTeam = Team;
            if (Team != null)
            {
                Team.TryRemoveMember(this);
                Team = null;
            }
            if (newTeam != null)
            {
                newTeam.TryAddMember(this);
                Team = newTeam;
            }
            OnTeamChanged?.Invoke(previousTeam, Team);
        }
        public void TryLeaveTeam()
        => TryChangeTeam(null);

        // Play
        protected override void PlayStart()
        {
            base.PlayStart();
            TryChangeTeam(StartingTeam);
        }
    }
}