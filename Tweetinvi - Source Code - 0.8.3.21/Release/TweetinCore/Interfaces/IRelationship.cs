namespace TweetinCore.Interfaces
{
    public interface IRelationship
    {
        IUser Source { get; }
        IUser Target { get; }

        bool Following { get; }
        bool FollowedBy { get; }
        bool NotificationsEnabled { get; }
        bool Blocking { get; }
        bool WantRetweets { get; }
        bool AllReplies { get; }
        bool MarkedSpam { get; }
        bool CanDm { get; }
    }
}
