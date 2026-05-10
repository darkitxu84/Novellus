namespace Novellus.Lib.Core.Plugins;

public record GameInfo(string Identifier, string Name);

public interface IGameSupport
{
    GameInfo Game { get; }
    Type ConfigType { get; }
    Type ManagerType { get; }
}