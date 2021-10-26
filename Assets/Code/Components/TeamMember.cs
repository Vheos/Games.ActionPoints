namespace Vheos.Games.ActionPoints
{
    using Tools.UnityCore;

    public class TeamMember : APlayable
    {
        // Inspector
        public Team _StartingTeam;

        // Publics
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

        // Mono
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