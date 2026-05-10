namespace Novellus.Lib.Core.Plugins;

public abstract class GameConfig
{
    [ConfigMetadata("Output Folder")] [ConfigPath] 
    public string OutputPath { get; set; } = "";
    
    public virtual bool Validate() => Path.Exists(OutputPath);
}

public abstract class Ps2GameConfig : GameConfig
{
    [ConfigMetadata("Game ISO Path")] [ConfigFile("PS2 Iso", "*.iso")] 
    public string ISOPath { get; set; } = "";
    
    [ConfigMetadata("Game Elf Path")] [ConfigFile("PS2 Executable", "*.elf")]
    public string ElfPath { get; set; } = "";
    
    [ConfigMetadata("PCSX2 Executable Path")] [ConfigFile("PCSX2 Executable")]
    public string PCSX2Path { get; set; } = "";
    
    [ConfigMetadata("PCSX2 Cheats Path")] [ConfigPath]
    public string CheatsPath { get; set; } = "";
    
    [ConfigMetadata("PCSX2 Textures Path")] [ConfigPath]
    public string TexturesPath { get; set; } = "";
    
    public bool UseNewPnachFormat { get; set; } = false;

    public override bool Validate()
    {
        return base.Validate();
    }
}

public abstract class PSPGameConfig : GameConfig
{
    [ConfigMetadata("Game ISO Path")] [ConfigFile("PSP Iso", "*.iso")] 
    public string ISOPath { get; set; } = "";
    
    [ConfigMetadata("PPSSPP Executable Path")] [ConfigFile("PPSSPP Executable")] 
    public string PPSSPPPath { get; set; } = "";
    
    [ConfigMetadata("PPSSPP Textures Path")] [ConfigPath]
    public string TexturesPath { get; set; } = "";
    
    [ConfigMetadata("PPSSPP Cheats Path")] [ConfigPath]
    public string CheatsPath { get; set; } = "";
    
    public override bool Validate()
    {
        return base.Validate();
    }
}

public abstract class N3DSGameConfig : GameConfig
{
    public string RomPath { get; set; } = "";
    public string EmuPath { get; set; } = "";
    public string TexturesPath { get; set; } = "";
    public string DataCpkPath { get; set; } = "";
    
    public override bool Validate()
    {
        return base.Validate();
    }
}
