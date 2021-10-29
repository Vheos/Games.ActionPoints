namespace Vheos.Games.ActionPoints
{
    using UnityEngine;

    public class Teamable : ABaseComponent
    {
        // Inspector
        [SerializeField] protected Predefined _StartingTeam;

        // Events
        public event System.Action<Team, Team> OnTeamChanged;

        // Publics
        public Team StartingTeam
        => _StartingTeam switch
        {
            Predefined.Players => Team.Players,
            Predefined.Enemies => Team.Enemies,
            _ => default,
        };
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

        // Play
        public override void PlayStart()
        {
            base.PlayStart();
            ChangeTeam(StartingTeam);
        }

        // Defines
        public enum Predefined
        {
            None,
            Players,
            Enemies,
        }
    }
}