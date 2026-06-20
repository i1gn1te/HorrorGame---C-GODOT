using Godot;

public partial class AudioMoodDirector : Node
{
    // Ten skrypt pilnuje tla dzwiekowego calej gry.
    // Gracz i HUD zostaja w Main.tscn, a poziomy tylko prosza dyrektora audio o zmiane klimatu.

    [Export] public NodePath HumPlayerPath = "HumLoop";
    [Export] public NodePath RumblePlayerPath = "RumbleLoop";
    [Export] public NodePath StingerPlayerPath = "StingerPlayer";

    [Export] public string HumStreamPath = "res://Assets/Audio/ambient_fluorescent_hum.wav";
    [Export] public string RumbleStreamPath = "res://Assets/Audio/ambient_low_rumble.wav";
    [Export] public string StingerStreamPath = "res://Assets/Audio/stinger_dark_presence.wav";
    [Export] public string FlashStreamPath = "res://Assets/Audio/Combat/flash_burst.wav";
    [Export] public string JumpscareStreamPath = "res://Assets/Audio/Combat/jumpscare_hit.wav";
    [Export] public string StoryWhisperStreamPath = "res://Assets/Audio/Ambient/story_wall_whisper.wav";

    [Export] public float HumVolumeDb = -18f;
    [Export] public float RumbleCalmVolumeDb = -34f;
    [Export] public float RumbleThreatVolumeDb = -20f;
    [Export] public float OneShotVolumeDb = -10f;

    private AudioStreamPlayer _humPlayer;
    private AudioStreamPlayer _rumblePlayer;
    private AudioStreamPlayer _stingerPlayer;
    private float _threatLevel;
    private bool _audioEnabled = true;
    private bool _humLoopConnected;
    private bool _rumbleLoopConnected;

    public override void _Ready()
    {
        AddToGroup("audio_director");

        _humPlayer = GetNodeOrNull<AudioStreamPlayer>(HumPlayerPath);
        _rumblePlayer = GetNodeOrNull<AudioStreamPlayer>(RumblePlayerPath);
        _stingerPlayer = GetNodeOrNull<AudioStreamPlayer>(StingerPlayerPath);

        if (IsHeadlessRun())
        {
            _audioEnabled = false;
            return;
        }

        LoadStream(_humPlayer, HumStreamPath);
        LoadStream(_rumblePlayer, RumbleStreamPath);
        LoadStream(_stingerPlayer, StingerStreamPath);

        SetupLoop(_humPlayer, HumVolumeDb);
        SetupLoop(_rumblePlayer, RumbleCalmVolumeDb);

        if (_humPlayer != null)
        {
            _humPlayer.Finished += OnHumFinished;
            _humLoopConnected = true;
        }

        if (_rumblePlayer != null)
        {
            _rumblePlayer.Finished += OnRumbleFinished;
            _rumbleLoopConnected = true;
        }
    }

    public override void _ExitTree()
    {
        if (_humPlayer != null)
        {
            if (_humLoopConnected)
            {
                _humPlayer.Finished -= OnHumFinished;
            }

            StopAndRelease(_humPlayer);
        }

        if (_rumblePlayer != null)
        {
            if (_rumbleLoopConnected)
            {
                _rumblePlayer.Finished -= OnRumbleFinished;
            }

            StopAndRelease(_rumblePlayer);
        }

        StopAndRelease(_stingerPlayer);
    }

    public void SetThreatLevel(float value)
    {
        // ThreatLevel 0 = spokoj, ThreatLevel 1 = mocny nacisk i wiecej basu.
        _threatLevel = Mathf.Clamp(value, 0f, 1f);

        if (_rumblePlayer != null)
        {
            _rumblePlayer.VolumeDb = Mathf.Lerp(RumbleCalmVolumeDb, RumbleThreatVolumeDb, _threatLevel);
            _rumblePlayer.PitchScale = Mathf.Lerp(0.92f, 1.06f, _threatLevel);
        }
    }

    public void SetAmbientProfile(string humPath, string rumblePath, float humVolumeDb, float calmRumbleDb, float threatRumbleDb, float threatLevel)
    {
        // Poziom moze wywolac te metode przez trigger przy starcie.
        // Dzieki temu Level 0 brzmi jak lampy, Level 2 jak rury, a Level 3 jak stacja elektryczna.
        HumStreamPath = humPath;
        RumbleStreamPath = rumblePath;
        HumVolumeDb = humVolumeDb;
        RumbleCalmVolumeDb = calmRumbleDb;
        RumbleThreatVolumeDb = threatRumbleDb;

        if (!_audioEnabled)
        {
            return;
        }

        LoadStream(_humPlayer, HumStreamPath);
        LoadStream(_rumblePlayer, RumbleStreamPath);
        SetupLoop(_humPlayer, HumVolumeDb);
        SetupLoop(_rumblePlayer, RumbleCalmVolumeDb);
        SetThreatLevel(threatLevel);
    }

    public void PlayStinger()
    {
        PlayOneShot(StingerStreamPath, OneShotVolumeDb);
    }

    public void PlayFlashBurst()
    {
        // Dzwiek reakcji gracza: krotki blysk i trzask powietrza.
        PlayOneShot(FlashStreamPath, OneShotVolumeDb - 2f);
    }

    public void PlayJumpscare()
    {
        // Dzwiek do naglego wejscia przeciwnika w kadr.
        PlayOneShot(JumpscareStreamPath, OneShotVolumeDb);
    }

    public void PlayStoryWhisper()
    {
        // Cichy szept do obrazow, map i notatek na scianach.
        PlayOneShot(StoryWhisperStreamPath, OneShotVolumeDb - 8f);
    }

    private void SetupLoop(AudioStreamPlayer player, float volumeDb)
    {
        if (player == null)
        {
            return;
        }

        player.VolumeDb = volumeDb;

        if (!player.Playing)
        {
            player.Play();
        }
    }

    private void LoadStream(AudioStreamPlayer player, string streamPath)
    {
        if (player == null || string.IsNullOrWhiteSpace(streamPath) || !ResourceLoader.Exists(streamPath))
        {
            return;
        }

        player.Stream = GD.Load<AudioStream>(streamPath);
    }

    private void PlayOneShot(string streamPath, float volumeDb)
    {
        if (!_audioEnabled || _stingerPlayer == null || string.IsNullOrWhiteSpace(streamPath) || !ResourceLoader.Exists(streamPath))
        {
            return;
        }

        _stingerPlayer.Stop();
        _stingerPlayer.Stream = GD.Load<AudioStream>(streamPath);
        _stingerPlayer.VolumeDb = volumeDb;
        _stingerPlayer.Play();
    }

    private bool IsHeadlessRun()
    {
        return DisplayServer.GetName().ToLower().Contains("headless");
    }

    private void OnHumFinished()
    {
        _humPlayer?.Play();
    }

    private void OnRumbleFinished()
    {
        _rumblePlayer?.Play();
    }

    private void StopAndRelease(AudioStreamPlayer player)
    {
        if (player == null)
        {
            return;
        }

        player.Stop();
        player.Stream = null;
    }
}
