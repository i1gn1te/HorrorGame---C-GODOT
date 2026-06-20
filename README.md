# Gra3D - Liminal First-Person Horror

Prototyp gry 3D w Godot 4.4 .NET/C#.

Kierunek: liniowy first-person horror akcji w swiecie liminal spaces. Grafika jest jeszcze z prostych klockow, bo najpierw budujemy ruch, rytm poziomow, strach, jumpscare, dzwiek i podstawowa mechanike reakcji.

## Jak uruchomic

1. Otworz folder `3d_souls-game` w Godot 4.4 .NET.
2. Uruchom scene `Scenes/Main.tscn`.
3. Sterowanie:
   - `WASD` - ruch
   - `Mysz` - kamera
   - `LPM` - szybki FlashBurst
   - `PPM` - mocniejszy FlashBurst
   - `Spacja` - unik
   - `Shift` - sprint
   - `Esc` - pokaz/schowaj kursor

## Grywalna trasa

- `Scenes/Levels/Level01_LiminalLobby.tscn` - jasny Level 0, duza pusta przestrzen, brak enemy.
- `Scenes/Levels/Level02_HabitableZone.tscn` - betonowa Habitable Zone, slupy i slady ludzi.
- `Scenes/Levels/Level03_UtilityHalls.tscn` - ciasne Utility Halls, rury i duzy jumpscare zza rogu.
- `Scenes/Levels/Level04_ElectricalStation.tscn` - elektryczna stacja, nacisk na FlashBurst.
- `Scenes/Levels/Level05_AbandonedOffice.tscn` - puste biuro, czarne okna i fabula przez obrazy.

## Najwazniejsze skrypty

- `Scripts/Player/PlayerController.cs` - ruch FPP, kamera, sprint i unik.
- `Scripts/Player/FlashBurstAbility.cs` - obecna mechanika walki/reakcji.
- `Scripts/Player/PlayerFear.cs` - pasek strachu.
- `Scripts/World/BackroomsLevelBuilder.cs` - proceduralne tworzenie nowych poziomow z klockow.
- `Scripts/World/JumpscareTrigger.cs` - aktywacja ukrytego enemy i skok strachu.
- `Scripts/World/WallStoryTrigger.cs` - fabula z obrazow na scianach.
- `Scripts/World/WindowTrapTrigger.cs` - czarne okna-pulapki.
- `Scripts/World/AudioProfileTrigger.cs` - zmiana ambientu po wejsciu w level.
- `Scripts/Core/AudioMoodDirector.cs` - warstwy audio, stinger, FlashBurst sound i whispers.

## Najwazniejsze dokumenty

- `Docs/BACKROOMS_CHRONOLOGICAL_ROUTE.md` - trasa Level 0 -> Level 4.
- `Docs/COMBAT_AND_JUMPSCARE_SYSTEM.md` - walka, strach i jumpscare.
- `Docs/LEVEL_CREATION_GUIDE.md` - jak tworzyc kolejne poziomy.
- `Docs/BLENDER_LEVEL0_FLOOR_FROM_ZERO.md` - podloga Level 0 w Blenderze od absolutnego zera.
- `Docs/LEVEL0_ESCAPE_BACKROOMS_TASK_QUEUE.md` - kolejka przebudowy Level 0 na haslo "dawaj robimy".
- `Docs/LEVEL0_WALLPAPER_VARIANTS.md` - warianty tapety i losowanie wzorow na scianach.
- `Docs/STORY_WALL_IMAGES.md` - creepy fabula przez obrazy.
- `Docs/ENEMY_ROSTER.md` - nowi przeciwnicy, modele i zachowania AI.
- `Docs/MODELING_LEARNING.md` - nauka modelingu i modele przykladowe.
- `Docs/SOUND_AND_FEAR_DIRECTION.md` - klimat audio i zasady strachu.

## Assety

- `Assets/Graphics/Level0/` - grafiki pierwszego poziomu.
- `Assets/Graphics/Level1/` - mapa i beton Level 1.
- `Assets/Graphics/Level2/` - mapa i rury Level 2.
- `Assets/Graphics/Level3/` - mapa i elektryka Level 3.
- `Assets/Graphics/Level4/` - mapa i biuro Level 4.
- `Assets/Graphics/Story/` - obrazy fabularne na sciany.
- `Assets/Audio/Combat/` - FlashBurst i jumpscare.
- `Assets/Audio/Ambient/` - drony poziomow i szept obrazow.
- `Models/Backrooms/` - nowe proste modele OBJ.

## Co jest juz grywalne

- Plynny ruch pierwszoosobowy.
- Kamera z oczu gracza.
- Sprint, stamina i unik.
- FlashBurst jako aktywna obrona.
- HUD zdrowia, staminy i strachu.
- Checkpoint i respawn.
- Liniowe przejscie przez kilka poziomow.
- Mapy i plakaty na scianach.
- Jumpscare zza rogu.
- Czarne okna-pulapki.
- Proceduralne poziomy z prostych bryl.
- Nowe typy enemy: Watcher Mannequin, Pipe Runner, Office Mimic i Corner Stalker.

## Kolejne dobre kroki

1. Dodac footstepy zalezne od podloza.
2. Dodac prosty head-bob i drzenie kamery przy wysokim strachu.
3. Zrobic pierwszy model `.glb` w Blenderze i podmienic nim placeholder.
4. Rozwinac FlashBurst o cooldown widoczny na HUD.
5. Dodac enemy, ktory reaguje na patrzenie gracza.
