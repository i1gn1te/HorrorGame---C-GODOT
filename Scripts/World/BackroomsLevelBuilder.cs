using Godot;

public partial class BackroomsLevelBuilder : Node3D
{
    // Ten builder tworzy proste poziomy z klockow.
    // Dzieki temu nie musisz jeszcze modelowac w Blenderze, a gra ma juz konkretna trase.
    // BackroomsLevel:
    // 1 = Habitable Zone, duza betonowa przestrzen.
    // 2 = Abandoned Utility Halls, ciasne rury i pierwszy duzy jumpscare.
    // 3 = Electrical Station, glosna elektryka i nacisk na FlashBurst.
    // 4 = Abandoned Office, puste biuro i czarne okna-pulapki.

    [Export] public int BackroomsLevel = 1;
    [Export] public string NextScenePath = "";
    [Export] public string EndMessage = "Dalsza czesc trasy powstanie w kolejnym kroku.";

    private Material _floorMaterial;
    private Material _wallMaterial;
    private Material _ceilingMaterial;
    private Material _exitMaterial;
    private Material _checkpointMaterial;
    private Material _mapPosterMaterial;
    private Material _storyPosterMaterial;
    private Material _darkPatchMaterial;

    public override void _Ready()
    {
        LoadMaterials();
        BuildEnvironment();

        switch (BackroomsLevel)
        {
            case 1:
                BuildHabitableZone();
                break;
            case 2:
                BuildUtilityHalls();
                break;
            case 3:
                BuildElectricalStation();
                break;
            case 4:
                BuildAbandonedOffice();
                break;
            default:
                BuildHabitableZone();
                break;
        }
    }

    private void LoadMaterials()
    {
        _exitMaterial = LoadMaterial("res://Materials/M_Exit.tres", new Color(0.5f, 0.9f, 1f));
        _checkpointMaterial = LoadMaterial("res://Materials/M_Checkpoint.tres", new Color(0.4f, 0.8f, 0.9f));
        _darkPatchMaterial = LoadMaterial("res://Materials/M_DarkPatch.tres", new Color(0.02f, 0.02f, 0.025f));
        _ceilingMaterial = LoadMaterial("res://Materials/M_Ceiling.tres", new Color(0.7f, 0.7f, 0.65f));

        string levelMaterialPath = BackroomsLevel switch
        {
            1 => "res://Materials/M_Level1_Concrete.tres",
            2 => "res://Materials/M_Level2_Pipes.tres",
            3 => "res://Materials/M_Level3_Electric.tres",
            4 => "res://Materials/M_Level4_Office.tres",
            _ => "res://Materials/M_Level1_Concrete.tres"
        };

        string mapPosterPath = BackroomsLevel switch
        {
            1 => "res://Materials/M_Level1_MapPoster.tres",
            2 => "res://Materials/M_Level2_MapPoster.tres",
            3 => "res://Materials/M_Level3_MapPoster.tres",
            4 => "res://Materials/M_Level4_MapPoster.tres",
            _ => "res://Materials/M_Level1_MapPoster.tres"
        };

        string storyPosterPath = BackroomsLevel switch
        {
            1 => "res://Materials/M_StoryPoster_02.tres",
            2 => "res://Materials/M_StoryPoster_03.tres",
            3 => "res://Materials/M_StoryPoster_04.tres",
            4 => "res://Materials/M_StoryPoster_05.tres",
            _ => "res://Materials/M_StoryPoster_02.tres"
        };

        _floorMaterial = LoadMaterial(levelMaterialPath, new Color(0.55f, 0.56f, 0.54f));
        _wallMaterial = LoadMaterial(levelMaterialPath, new Color(0.55f, 0.56f, 0.54f));
        _mapPosterMaterial = LoadMaterial(mapPosterPath, new Color(0.8f, 0.8f, 0.8f));
        _storyPosterMaterial = LoadMaterial(storyPosterPath, new Color(0.85f, 0.78f, 0.65f));
    }

    private void BuildEnvironment()
    {
        ProceduralSkyMaterial skyMaterial = new ProceduralSkyMaterial();
        skyMaterial.SkyHorizonColor = BackroomsLevel switch
        {
            1 => new Color(0.12f, 0.13f, 0.13f),
            2 => new Color(0.045f, 0.04f, 0.035f),
            3 => new Color(0.025f, 0.03f, 0.045f),
            4 => new Color(0.055f, 0.058f, 0.06f),
            _ => new Color(0.1f, 0.1f, 0.1f)
        };
        skyMaterial.GroundHorizonColor = skyMaterial.SkyHorizonColor;

        Sky sky = new Sky();
        sky.SkyMaterial = skyMaterial;

        Environment environment = new Environment();
        environment.BackgroundMode = Environment.BGMode.Sky;
        environment.Sky = sky;
        environment.AmbientLightSource = Environment.AmbientSource.Color;
        environment.AmbientLightColor = BackroomsLevel switch
        {
            1 => new Color(0.38f, 0.40f, 0.38f),
            2 => new Color(0.12f, 0.10f, 0.08f),
            3 => new Color(0.07f, 0.09f, 0.13f),
            4 => new Color(0.16f, 0.17f, 0.17f),
            _ => new Color(0.2f, 0.2f, 0.2f)
        };
        environment.AmbientLightEnergy = BackroomsLevel == 1 ? 0.95f : 0.45f;
        environment.VolumetricFogEnabled = true;
        environment.VolumetricFogDensity = BackroomsLevel switch
        {
            1 => 0.01f,
            2 => 0.035f,
            3 => 0.045f,
            4 => 0.018f,
            _ => 0.01f
        };

        WorldEnvironment worldEnvironment = new WorldEnvironment();
        worldEnvironment.Name = "WorldEnvironment";
        worldEnvironment.Environment = environment;
        AddChild(worldEnvironment);
    }

    private void BuildHabitableZone()
    {
        AddPlayerSpawn(new Vector3(0f, 0.1f, 14f));
        AddAudioProfile("res://Assets/Audio/ambient_fluorescent_hum.wav", "res://Assets/Audio/ambient_low_rumble.wav", -20f, -36f, -22f, 0.15f);
        AddRoomShell("HabitableShell", new Vector2(30f, 38f), 3.4f);

        // Duze puste miejsce i rytmiczne slupy. To ma dac oddech po Level 0.
        for (int x = -10; x <= 10; x += 5)
        {
            for (int z = -9; z <= 8; z += 6)
            {
                AddBox($"ConcretePillar_{x}_{z}", new Vector3(x, 1.7f, z), new Vector3(1.15f, 3.4f, 1.15f), _wallMaterial);
            }
        }

        AddBox("BaseCamp_Block_Left", new Vector3(-10.5f, 1.2f, 10.5f), new Vector3(5f, 2.4f, 4f), _wallMaterial);
        AddBox("BaseCamp_Block_Right", new Vector3(9.5f, 1.2f, 5f), new Vector3(4.5f, 2.4f, 6f), _wallMaterial);
        AddBox("OpenParking_BrokenDivider", new Vector3(0f, 0.6f, -3.5f), new Vector3(9f, 1.2f, 0.35f), _wallMaterial);
        AddBox("FarEmptyWallPatch", new Vector3(-4f, 1.65f, -18.2f), new Vector3(4f, 2.8f, 0.08f), _darkPatchMaterial, false);

        AddLight("SoftFlood_01", new Vector3(-8f, 3.05f, 10f), 2.3f, 13f, new Color(0.8f, 0.93f, 1f), false);
        AddLight("SoftFlood_02", new Vector3(7f, 3.05f, 3f), 2.0f, 13f, new Color(0.82f, 0.92f, 1f), false);
        AddLight("BrokenLamp_Far", new Vector3(-3f, 3.05f, -13f), 0.55f, 5f, new Color(0.65f, 0.78f, 1f), true);

        AddPosterPair(
            new Vector3(-8f, 1.75f, 18.35f),
            new Vector3(3.8f, 1.75f, 18.35f),
            "Na zdjeciu widac ludzi przy ognisku z jarzeniowek. Ich twarze sa zamazane, ale twoja kurtka wisi za nimi.",
            8f,
            0.25f
        );

        AddCheckpoint(new Vector3(0f, 1.1f, 12f), "Po raz pierwszy od dawna przestrzen wyglada, jakby ktos tu mieszkal.");
        AddFearZone(new Vector3(-4f, 1.2f, -12f), new Vector3(5f, 2.2f, 4f), "Ciemny fragment sciany nie odbija swiatla.", 10f);
        AddWatcherMannequin(new Vector3(-3.6f, 0f, -13.8f), "Manekin przy ciemnej scianie");
        AddExit(new Vector3(0f, 1.35f, -18.1f), "Wejscie do hal technicznych jeszcze nie istnieje.");
    }

    private void BuildUtilityHalls()
    {
        AddPlayerSpawn(new Vector3(0f, 0.1f, 15f));
        AddAudioProfile("res://Assets/Audio/ambient_fluorescent_hum.wav", "res://Assets/Audio/Ambient/industrial_pipe_drone.wav", -24f, -30f, -16f, 0.35f);
        AddRoomShell("UtilityShell", new Vector2(13f, 42f), 2.8f);

        // Ciasne bloki robia zakrety. Jumpscare ma wejsc zza rogu, nie z pustej przestrzeni.
        AddBox("TurnBlock_Left_A", new Vector3(-3.9f, 1.4f, 7f), new Vector3(4.3f, 2.8f, 10f), _wallMaterial);
        AddBox("TurnBlock_Right_A", new Vector3(4.1f, 1.4f, -2f), new Vector3(4.2f, 2.8f, 8f), _wallMaterial);
        AddBox("DeadServiceRoom", new Vector3(-4.2f, 1.4f, -12.5f), new Vector3(3.8f, 2.8f, 4.6f), _wallMaterial);
        AddBox("PipeWallShadow", new Vector3(6.2f, 1.35f, -8.5f), new Vector3(0.12f, 2.2f, 4f), _darkPatchMaterial, false);

        for (int z = 12; z >= -16; z -= 4)
        {
            AddBox($"LeftPipe_{z}", new Vector3(-6.32f, 2.15f, z), new Vector3(0.18f, 0.18f, 2.8f), _wallMaterial, false);
            AddBox($"RightPipe_{z}", new Vector3(6.32f, 2.0f, z - 1f), new Vector3(0.18f, 0.18f, 2.8f), _wallMaterial, false);
        }

        AddLight("Brownout_Entry", new Vector3(2.5f, 2.45f, 11f), 0.9f, 6.5f, new Color(0.9f, 0.72f, 0.45f), true);
        AddLight("Brownout_Middle", new Vector3(-2.5f, 2.45f, -1.5f), 0.65f, 5.5f, new Color(0.8f, 0.62f, 0.38f), true);
        AddLight("Brownout_Corner", new Vector3(4.6f, 2.45f, -8f), 0.32f, 4.5f, new Color(0.65f, 0.48f, 0.3f), true);

        AddPosterPair(
            new Vector3(-4.7f, 1.5f, 20.8f),
            new Vector3(4.2f, 1.5f, 20.8f),
            "Ktos narysowal plan rur. Jedna linia podpisana jest twoim imieniem i konczy sie przy tym zakrecie.",
            14f,
            0.45f
        );

        AddCheckpoint(new Vector3(0f, 1.1f, 12.5f), "Rury stukaja wolniej, kiedy stoisz bez ruchu.");
        AddFearZone(new Vector3(0f, 1.2f, 4.2f), new Vector3(4f, 2.2f, 4f), "Zza sciany dochodzi drapanie, ale tylko wtedy, gdy idziesz.", 14f);
        AddHiddenCornerStalker(new Vector3(5.25f, 0f, -5.7f), new Vector3(1.2f, 1.2f, -1.2f), "Wielki ksztalt wypada zza rogu. Blysk albo uciekaj.", 42f);
        AddPipeRunner(new Vector3(-2.2f, 0f, -13.4f), "Pipe Runner w serwisowym tunelu");
        AddExit(new Vector3(0f, 1.35f, -20.1f), "Drzwi do stacji elektrycznej jeszcze nie istnieja.");
    }

    private void BuildElectricalStation()
    {
        AddPlayerSpawn(new Vector3(0f, 0.1f, 13f));
        AddAudioProfile("res://Assets/Audio/Ambient/electric_station_loop.wav", "res://Assets/Audio/ambient_low_rumble.wav", -18f, -28f, -14f, 0.55f);
        AddRoomShell("ElectricalShell", new Vector2(16f, 38f), 2.9f);

        AddBox("NarrowCore_A", new Vector3(-4.2f, 1.45f, 6f), new Vector3(4.2f, 2.9f, 10f), _wallMaterial);
        AddBox("NarrowCore_B", new Vector3(4.3f, 1.45f, -4.5f), new Vector3(4.4f, 2.9f, 9f), _wallMaterial);
        AddBox("MachineBlock_A", new Vector3(-4.8f, 1.0f, -13.5f), new Vector3(3f, 2f, 4f), _wallMaterial);
        AddBox("MachineBlock_B", new Vector3(5f, 1.0f, 10.5f), new Vector3(3f, 2f, 4f), _wallMaterial);

        for (int z = 11; z >= -15; z -= 5)
        {
            AddBox($"ElectricPanel_L_{z}", new Vector3(-7.82f, 1.45f, z), new Vector3(0.12f, 1.8f, 2.2f), _wallMaterial, false);
            AddBox($"ElectricPanel_R_{z}", new Vector3(7.82f, 1.45f, z - 2f), new Vector3(0.12f, 1.8f, 2.2f), _wallMaterial, false);
        }

        AddLight("BlueArc_Entry", new Vector3(0f, 2.55f, 10f), 0.95f, 6.5f, new Color(0.45f, 0.65f, 1f), true);
        AddLight("BlueArc_Middle", new Vector3(2f, 2.55f, -3f), 1.05f, 6.5f, new Color(0.35f, 0.55f, 1f), true);
        AddLight("BlueArc_Exit", new Vector3(-1f, 2.55f, -14f), 0.75f, 5.5f, new Color(0.45f, 0.72f, 1f), true);

        AddPosterPair(
            new Vector3(-5.0f, 1.58f, 18.35f),
            new Vector3(4.6f, 1.58f, 18.35f),
            "Fotografia jest przepalona blyskiem. W bialej plamie widac sylwetke, ktora podnosi reke tak jak ty.",
            18f,
            0.6f
        );

        AddCheckpoint(new Vector3(0f, 1.1f, 11.5f), "Elektryka na chwile zapamietuje twoj rytm oddechu.");
        AddFearZone(new Vector3(0f, 1.2f, -2f), new Vector3(5f, 2.2f, 5f), "Cale pomieszczenie trzaska, jakby ktos naciskal z drugiej strony scian.", 18f);
        AddPipeRunner(new Vector3(0f, 0f, -9.5f), "Przepalony Pipe Runner");
        AddWatcherMannequin(new Vector3(-5.2f, 0f, -14.2f), "Manekin pod panelem");
        AddExit(new Vector3(0f, 1.35f, -18.1f), "Puste biuro jeszcze czeka na scene.");
    }

    private void BuildAbandonedOffice()
    {
        AddPlayerSpawn(new Vector3(0f, 0.1f, 12f));
        AddAudioProfile("res://Assets/Audio/Ambient/office_silence_tone.wav", "res://Assets/Audio/ambient_low_rumble.wav", -20f, -38f, -20f, 0.35f);
        AddRoomShell("OfficeShell", new Vector2(24f, 34f), 3.1f);

        // Puste boksy biurowe. Im wiecej scianek, tym bardziej gracz patrzy w okna.
        for (int x = -7; x <= 7; x += 7)
        {
            AddBox($"CubicleWall_A_{x}", new Vector3(x, 0.95f, 4f), new Vector3(0.22f, 1.9f, 7f), _wallMaterial);
            AddBox($"CubicleWall_B_{x}", new Vector3(x + 2.8f, 0.95f, -5f), new Vector3(0.22f, 1.9f, 6f), _wallMaterial);
        }

        AddBox("ReceptionBlock", new Vector3(-6.5f, 0.8f, 10f), new Vector3(4f, 1.6f, 2.2f), _wallMaterial);
        AddBox("ArchiveBlock", new Vector3(7.5f, 1.55f, -10.5f), new Vector3(3.5f, 3.1f, 5f), _wallMaterial);

        AddWindowTrap(new Vector3(-11.85f, 1.55f, 3.5f), "Czarne okno pokazuje korytarz, ktorego nie ma za ta sciana.");
        AddWindowTrap(new Vector3(11.85f, 1.55f, -4.5f), "W odbiciu stoisz nieruchomo, chociaz wlasnie sie poruszyles.");
        AddWindowTrap(new Vector3(-4f, 1.55f, -16.85f), "Za szyba ktos przykleil zdjecie pokoju, do ktorego jeszcze nie wszedles.");

        AddLight("OfficeLamp_Entry", new Vector3(0f, 2.85f, 8f), 1.15f, 8f, new Color(0.88f, 0.92f, 1f), false);
        AddLight("OfficeLamp_Middle", new Vector3(-5f, 2.85f, -2f), 0.8f, 6f, new Color(0.75f, 0.82f, 1f), true);
        AddLight("OfficeLamp_Exit", new Vector3(4f, 2.85f, -12f), 0.5f, 5f, new Color(0.65f, 0.72f, 0.95f), true);

        AddPosterPair(
            new Vector3(-5.5f, 1.65f, 16.85f),
            new Vector3(5.5f, 1.65f, 16.85f),
            "Ostatni obraz nie jest mapa. To instrukcja ewakuacji z twojego mieszkania.",
            20f,
            0.55f
        );

        AddCheckpoint(new Vector3(0f, 1.1f, 10f), "Biuro zapamietuje cie jak pracownika, ktory nigdy nie wyszedl.");
        AddFearZone(new Vector3(0f, 1.2f, -8f), new Vector3(6f, 2.2f, 5f), "Telefony sa martwe. Mimo tego wszystkie wskazuja twoj numer.", 18f);
        AddOfficeMimic(new Vector3(4.2f, 0f, -5.8f), "Krzeslo, ktore oddycha");
        AddOfficeMimic(new Vector3(-7.6f, 0f, 5.7f), "Czarny fotel przy recepcji");
        AddExit(new Vector3(0f, 1.35f, -16.1f), "To koniec obecnej trasy. Dalej zacznie sie nowy rozdzial.");
    }

    private void AddRoomShell(string name, Vector2 size, float height)
    {
        Node3D shell = new Node3D();
        shell.Name = name;
        AddChild(shell);

        float halfX = size.X * 0.5f;
        float halfZ = size.Y * 0.5f;

        AddBox("Floor", new Vector3(0f, -0.05f, 0f), new Vector3(size.X, 0.25f, size.Y), _floorMaterial, true, shell);
        AddBox("Ceiling", new Vector3(0f, height, 0f), new Vector3(size.X, 0.25f, size.Y), _ceilingMaterial, true, shell);
        AddBox("Wall_Left", new Vector3(-halfX, height * 0.5f, 0f), new Vector3(0.35f, height, size.Y), _wallMaterial, true, shell);
        AddBox("Wall_Right", new Vector3(halfX, height * 0.5f, 0f), new Vector3(0.35f, height, size.Y), _wallMaterial, true, shell);
        AddBox("Wall_Back", new Vector3(0f, height * 0.5f, halfZ), new Vector3(size.X, height, 0.35f), _wallMaterial, true, shell);
        AddBox("Wall_Front_Left", new Vector3(-halfX * 0.55f, height * 0.5f, -halfZ), new Vector3(size.X * 0.42f, height, 0.35f), _wallMaterial, true, shell);
        AddBox("Wall_Front_Right", new Vector3(halfX * 0.55f, height * 0.5f, -halfZ), new Vector3(size.X * 0.42f, height, 0.35f), _wallMaterial, true, shell);
    }

    private void AddPosterPair(Vector3 mapPosition, Vector3 storyPosition, string storyText, float fearAmount, float threatLevel)
    {
        AddPoster("MapPoster", mapPosition, _mapPosterMaterial);
        AddPoster("StoryPoster", storyPosition, _storyPosterMaterial);

        WallStoryTrigger storyTrigger = new WallStoryTrigger();
        storyTrigger.Name = "StoryTrigger";
        storyTrigger.StoryText = storyText;
        storyTrigger.FearAmount = fearAmount;
        storyTrigger.ThreatLevel = threatLevel;
        AddArea(storyTrigger, storyPosition + new Vector3(0f, -0.4f, -3.2f), new Vector3(5f, 2.4f, 4f));
    }

    private void AddAudioProfile(string humPath, string rumblePath, float humDb, float calmDb, float threatDb, float startThreat)
    {
        AudioProfileTrigger trigger = new AudioProfileTrigger();
        trigger.Name = "AudioProfile_Entry";
        trigger.HumStreamPath = humPath;
        trigger.RumbleStreamPath = rumblePath;
        trigger.HumVolumeDb = humDb;
        trigger.RumbleCalmVolumeDb = calmDb;
        trigger.RumbleThreatVolumeDb = threatDb;
        trigger.StartThreatLevel = startThreat;
        AddArea(trigger, new Vector3(0f, 1.1f, 12f), new Vector3(5f, 2.2f, 5f));
    }

    private void AddCheckpoint(Vector3 position, string message)
    {
        CheckpointTrigger checkpoint = new CheckpointTrigger();
        checkpoint.Name = "Checkpoint";
        checkpoint.CheckpointMessage = message;
        AddArea(checkpoint, position, new Vector3(2.8f, 2.2f, 2.8f));
        AddBox("Checkpoint_Visual", position + new Vector3(0f, -1.05f, 0f), new Vector3(1.2f, 0.08f, 1.2f), _checkpointMaterial, false);
    }

    private void AddFearZone(Vector3 position, Vector3 size, string message, float fearAmount)
    {
        FearZoneTrigger fearZone = new FearZoneTrigger();
        fearZone.Name = "FearZone";
        fearZone.Message = message;
        fearZone.FearAmount = fearAmount;
        AddArea(fearZone, position, size);
    }

    private void AddExit(Vector3 position, string missingMessage)
    {
        AddBox("Exit_Glow_Box", position + new Vector3(0f, 0f, -0.25f), new Vector3(2.6f, 2.4f, 0.22f), _exitMaterial, false);

        LevelExitTrigger exit = new LevelExitTrigger();
        exit.Name = "ExitTrigger";
        exit.NextScenePath = NextScenePath;
        exit.MissingSceneMessage = string.IsNullOrWhiteSpace(NextScenePath) ? EndMessage : missingMessage;
        AddArea(exit, position, new Vector3(3.2f, 2.8f, 1.2f));
    }

    private void AddHiddenJumpscareEnemy(Vector3 enemyPosition, Vector3 triggerPosition, string message, float fearAmount)
    {
        EnemyDummy enemy = AddEnemy(enemyPosition, "Wielki ksztalt zza rogu", 240f, 3.3f);
        enemy.StartHidden = true;
        enemy.StartInactive = true;
        enemy.Visible = false;
        enemy.Scale = new Vector3(1.75f, 1.75f, 1.75f);

        JumpscareTrigger jumpscare = new JumpscareTrigger();
        jumpscare.Name = "Jumpscare_BigCorner";
        jumpscare.Message = message;
        jumpscare.FearAmount = fearAmount;
        jumpscare.ThreatLevel = 1f;
        AddArea(jumpscare, triggerPosition, new Vector3(4.2f, 2.4f, 3.2f));
        jumpscare.EnemyPath = jumpscare.GetPathTo(enemy);
    }

    private void AddHiddenCornerStalker(Vector3 enemyPosition, Vector3 triggerPosition, string message, float fearAmount)
    {
        // Nowa wersja duzego jumpscare enemy.
        // Uzywa osobnej sceny i modelu, ale nadal odpala sie przez JumpscareTrigger.
        CornerStalkerEnemy enemy = AddEnemyScene<CornerStalkerEnemy>(
            "res://Scenes/Enemies/CornerStalkerEnemy.tscn",
            enemyPosition,
            "CornerStalker_Jumpscare"
        );

        enemy.StartHidden = true;
        enemy.StartInactive = true;
        enemy.Visible = false;
        enemy.AggroRange = 18f;

        JumpscareTrigger jumpscare = new JumpscareTrigger();
        jumpscare.Name = "Jumpscare_CornerStalker";
        jumpscare.Message = message;
        jumpscare.FearAmount = fearAmount;
        jumpscare.ThreatLevel = 1f;
        AddArea(jumpscare, triggerPosition, new Vector3(4.2f, 2.4f, 3.2f));
        jumpscare.EnemyPath = jumpscare.GetPathTo(enemy);
    }

    private void AddActiveEnemy(Vector3 position, string enemyName, float health, float speed)
    {
        EnemyDummy enemy = AddEnemy(position, enemyName, health, speed);
        enemy.AggroRange = 10f;
        enemy.AttackDamage = 14f;
    }

    private void AddWatcherMannequin(Vector3 position, string enemyName)
    {
        WatcherMannequinEnemy enemy = AddEnemyScene<WatcherMannequinEnemy>(
            "res://Scenes/Enemies/WatcherMannequinEnemy.tscn",
            position,
            enemyName.Replace(" ", "_")
        );

        enemy.EnemyName = enemyName;
        enemy.AggroRange = 13.5f;
        enemy.AttackDamage = 10f;
    }

    private void AddPipeRunner(Vector3 position, string enemyName)
    {
        PipeRunnerEnemy enemy = AddEnemyScene<PipeRunnerEnemy>(
            "res://Scenes/Enemies/PipeRunnerEnemy.tscn",
            position,
            enemyName.Replace(" ", "_")
        );

        enemy.EnemyName = enemyName;
        enemy.AggroRange = 15f;
        enemy.AttackDamage = 15f;
    }

    private void AddOfficeMimic(Vector3 position, string enemyName)
    {
        OfficeMimicEnemy enemy = AddEnemyScene<OfficeMimicEnemy>(
            "res://Scenes/Enemies/OfficeMimicEnemy.tscn",
            position,
            enemyName.Replace(" ", "_")
        );

        enemy.EnemyName = enemyName;
        enemy.AggroRange = 11.5f;
        enemy.FearPerSecond = 7.5f;
    }

    private EnemyDummy AddEnemy(Vector3 position, string enemyName, float health, float speed)
    {
        PackedScene enemyScene = GD.Load<PackedScene>("res://Scenes/Enemies/EnemyDummy.tscn");
        EnemyDummy enemy = enemyScene.Instantiate<EnemyDummy>();
        enemy.Name = enemyName.Replace(" ", "_");
        enemy.Position = position;
        enemy.EnemyName = enemyName;
        enemy.MaxHealth = health;
        enemy.MoveSpeed = speed;
        AddChild(enemy);
        return enemy;
    }

    private T AddEnemyScene<T>(string scenePath, Vector3 position, string nodeName) where T : Node3D
    {
        // Uniwersalne wstawianie nowych przeciwnikow.
        // Jesli pozniej zrobisz nowego enemy w Blenderze i osobna scene, dodasz tu tylko sciezke.
        PackedScene enemyScene = GD.Load<PackedScene>(scenePath);
        T enemy = enemyScene.Instantiate<T>();
        enemy.Name = nodeName;
        enemy.Position = position;
        AddChild(enemy);
        return enemy;
    }

    private void AddWindowTrap(Vector3 position, string message)
    {
        AddBox("BlackWindow", position, new Vector3(0.12f, 1.7f, 2.6f), _darkPatchMaterial, false);

        WindowTrapTrigger trap = new WindowTrapTrigger();
        trap.Name = "WindowTrap";
        trap.Message = message;
        AddArea(trap, position + new Vector3(position.X < 0f ? 1.8f : -1.8f, 0f, 0f), new Vector3(3f, 2.2f, 3.2f));
    }

    private void AddPlayerSpawn(Vector3 position)
    {
        Marker3D spawn = new Marker3D();
        spawn.Name = "PlayerSpawn";
        spawn.Position = position;
        AddChild(spawn);
    }

    private void AddPoster(string name, Vector3 position, Material material)
    {
        MeshInstance3D poster = new MeshInstance3D();
        poster.Name = name;
        poster.Position = position;
        poster.Mesh = new QuadMesh
        {
            Size = new Vector2(3.0f, 1.9f)
        };
        poster.SetSurfaceOverrideMaterial(0, material);
        AddChild(poster);
    }

    private void AddLight(string name, Vector3 position, float energy, float range, Color color, bool flicker)
    {
        OmniLight3D light = flicker ? new FlickerLight() : new OmniLight3D();
        light.Name = name;
        light.Position = position;
        light.LightColor = color;
        light.LightEnergy = energy;
        light.OmniRange = range;

        if (light is FlickerLight flickerLight)
        {
            flickerLight.BaseEnergy = energy;
            flickerLight.FlickerStrength = energy * 0.35f;
            flickerLight.RandomDropChance = 0.035f;
        }

        AddChild(light);
    }

    private Area3D AddArea(Area3D area, Vector3 position, Vector3 size)
    {
        area.Position = position;

        CollisionShape3D shape = new CollisionShape3D();
        shape.Shape = new BoxShape3D
        {
            Size = size
        };
        area.AddChild(shape);

        AddChild(area);
        return area;
    }

    private CsgBox3D AddBox(string name, Vector3 position, Vector3 size, Material material, bool useCollision = true, Node parent = null)
    {
        CsgBox3D box = new CsgBox3D();
        box.Name = name;
        box.Position = position;
        box.Size = size;
        box.UseCollision = useCollision;
        box.Material = material;

        if (parent == null)
        {
            AddChild(box);
        }
        else
        {
            parent.AddChild(box);
        }

        return box;
    }

    private Material LoadMaterial(string path, Color fallbackColor)
    {
        if (ResourceLoader.Exists(path))
        {
            return GD.Load<Material>(path);
        }

        StandardMaterial3D fallback = new StandardMaterial3D();
        fallback.AlbedoColor = fallbackColor;
        fallback.Roughness = 0.9f;
        return fallback;
    }
}
