namespace Vheos.Games.ActionPoints
{
    using UnityEngine;
    using Tools.UnityCore;

    public class TeamMember : ABaseComponent
    {
        // Inspector
        [SerializeField] protected Team _StartingTeam;

        // Publics
        public Team StartingTeam
        => _StartingTeam;
        public Team Team
        { get; private set; }
        public void ChangeTeam(Team newTeam)
        {
            LeaveTeam();
            JoinTeam(newTeam);
        }
        public void LeaveTeam()
        {
            if (Team != null)
                Team.RemoveMember(this);
        }

        // Privates
        private void JoinTeam(Team newTeam)
        {
            if (newTeam == null)
                return;

            Team = newTeam;
            Team.TryInitialize();
            Team.AddMember(this);
        }

        // Play
        public override void PlayEnable()
        {
            base.PlayEnable();
            if (_StartingTeam != null)
                JoinTeam(_StartingTeam);
        }
        public override void PlayDisable()
        {
            base.PlayDisable();
            LeaveTeam();
        }
    }
}