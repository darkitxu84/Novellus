using System.ComponentModel;

namespace Novellus.Lib.Core.Plugins;

public abstract class GameConfig
{
    [DisplayName("Output Path")]
    [Description("Path to build output")]
    [ConfigPath] 
    public string OutputPath { get; set; } = "";
    
    public virtual bool Validate() => Path.Exists(OutputPath);
}

public abstract class Ps2GameConfig : GameConfig
{
    [DisplayName("Game ISO Path")]
    [Description("Path to the game ISO")]
    [ConfigFile("PS2 Iso", "*.iso")] 
    public string ISOPath { get; set; } = "";
    
    [DisplayName("Game ELF Executable")]
    [Description("Path to the game ELF executable")]
    [ConfigFile("PS2 Executable", "*.elf")]
    public string ElfPath { get; set; } = "";
    
    [DisplayName("PCSX2 Executable Path")]
    [Description("Path to the PCSX2 executable")]
    [ConfigFile("PCSX2 Executable")]
    public string PCSX2Path { get; set; } = "";
    
    [DisplayName("PCSX2 Cheats Path")]
    [Description("Path to the PCSX2 cheats folder")]
    [ConfigPath]
    public string CheatsPath { get; set; } = "";
    
    [DisplayName("PCSX2 Textures Path")]
    [Description("Path to the PCSX2 textures folder")]
    [ConfigPath]
    public string TexturesPath { get; set; } = "";
    
    [DisplayName("Use PNACH 2.0 for cheats")]
    [Description("Merge the cheats into a single cheat file")]
    public bool UseNewPnachFormat { get; set; } = false;

    public override bool Validate()
    {
        return base.Validate();
    }
}

public abstract class PSPGameConfig : GameConfig
{
    [DisplayName("Game ISO Path")]
    [ConfigFile("PSP Iso", "*.iso")] 
    public string ISOPath { get; set; } = "";
    
    [DisplayName("PPSSP Executable Path")]
    [ConfigFile("PPSSPP Executable")] 
    public string PPSSPPPath { get; set; } = "";
    
    [DisplayName("PPSSP Textures Path")]
    [ConfigPath]
    public string TexturesPath { get; set; } = "";
    
    [DisplayName("PPSSP Cheats Path")]
    [ConfigPath]
    public string CheatsPath { get; set; } = "";
    
    public override bool Validate()
    {
        return base.Validate();
    }
}

public abstract class N3DSGameConfig : GameConfig
{
    [DisplayName("Game ROM Path")]
    [Description("Path to the game ROM")]
    [ConfigFile("N3DS ROM", "*.3ds;*.app;*.cxi;*.cci")]
    public string RomPath { get; set; } = "";
    
    [DisplayName("3DS Emulator Executable Path")]
    [Description("Path to the 3DS Emulator executable")]
    [ConfigFile("3DS Emulator Executable")]
    public string EmuPath { get; set; } = "";
    
    [DisplayName("Textures Path")]
    [Description("Path to the 3DS Emulator textures folder")]
    [ConfigPath]
    public string TexturesPath { get; set; } = "";
    
    [DisplayName("DataCpk")]
    [ConfigFile("CPK", "*.cpk")]
    public string DataCpkPath { get; set; } = "";
    
    public override bool Validate()
    {
        return base.Validate();
    }
}
