# Gra3D - fundament projektu

## Zamysl

Gracz trafia do Backrooms/liminal spaces. Fabula jest liniowa: kazdy nastepny poziom robi sie bardziej dziwny, ciasny albo psychologicznie agresywny.

Aktualny kierunek:

- first-person horror,
- proste klockowe poziomy jako blockout,
- narastajaca cisza,
- obrazy fabularne na scianach,
- FlashBurst jako aktywna obrona,
- jumpscare, ktory prowadzi do gameplayu, nie tylko do halasu.

Inspiracja klimatu:

- Backrooms Wiki: https://backrooms-wiki.wikidot.com/
- From The Darkness jako kierunek ciszy, ciasnoty i starego, obserwujacego miejsca.

## Co jest w projekcie

Sceny:

- `Scenes/Main.tscn`
- `Scenes/Player/Player.tscn`
- `Scenes/Levels/Level01_LiminalLobby.tscn`
- `Scenes/Levels/Level02_HabitableZone.tscn`
- `Scenes/Levels/Level03_UtilityHalls.tscn`
- `Scenes/Levels/Level04_ElectricalStation.tscn`
- `Scenes/Levels/Level05_AbandonedOffice.tscn`

Skrypty gracza:

- `Scripts/Player/PlayerController.cs`
- `Scripts/Player/PlayerStats.cs`
- `Scripts/Player/PlayerFear.cs`
- `Scripts/Player/FlashBurstAbility.cs`

Skrypty swiata:

- `Scripts/World/BackroomsLevelBuilder.cs`
- `Scripts/World/LevelExitTrigger.cs`
- `Scripts/World/JumpscareTrigger.cs`
- `Scripts/World/FearZoneTrigger.cs`
- `Scripts/World/WallStoryTrigger.cs`
- `Scripts/World/WindowTrapTrigger.cs`
- `Scripts/World/AudioProfileTrigger.cs`

Core:

- `Scripts/Core/GameBootstrap.cs`
- `Scripts/Core/GameHud.cs`
- `Scripts/Core/AudioMoodDirector.cs`

## Sterowanie

- `WASD` - ruch.
- `Mysz` - kamera.
- `LPM` - szybki FlashBurst.
- `PPM` - mocniejszy FlashBurst.
- `Spacja` - unik.
- `Shift` - sprint.
- `Esc` - pokaz albo schowaj kursor.

## Zasada prototypowania

Najpierw grywalnosc na klockach. Potem modele z Blendera. Potem tekstury, animacje i polish.

Kolejnosc pracy:

1. Zbuduj korytarz jako blockout.
2. Ustaw swiatlo i audio.
3. Dodaj trigger strachu albo historii.
4. Sprawdz, czy gracz czuje napiecie bez enemy.
5. Dopiero potem dodaj enemy albo jumpscare.
6. Na koncu podmien bryly na modele.

## Najblizsze dobre kroki

1. Dodac footstepy.
2. Dodac drzenie kamery przy wysokim strachu.
3. Dodac prosty model `.glb` z Blendera do jednego poziomu.
4. Dodac UI cooldownu FlashBurst.
5. Rozwinac fabule obrazow tak, by jeden z nich klamal.
