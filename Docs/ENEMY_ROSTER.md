# Roster przeciwnikow

Ten plik opisuje nowych enemy. Kazdy z nich ma osobny skrypt C#, osobna scene i prosty model OBJ.

## Wspolny fundament

- `Scripts/Enemies/HorrorEnemyBase.cs`
  - zdrowie,
  - grawitacja,
  - stun po FlashBurst,
  - aktywacja przez jumpscare,
  - proste zadawanie obrazen.

- `Scripts/Core/IStunnable.cs`
  - pozwala FlashBurst ogluszac rozne typy enemy.

- `Scripts/Core/IJumpscareActivatable.cs`
  - pozwala `JumpscareTrigger` budzic ukrytego enemy.

## Watcher Mannequin

Scena:

- `Scenes/Enemies/WatcherMannequinEnemy.tscn`

Skrypt:

- `Scripts/Enemies/WatcherMannequinEnemy.cs`

Model:

- `Models/Backrooms/watcher_mannequin_tall.obj`

Zachowanie:

- rusza, gdy gracz nie patrzy,
- zatrzymuje sie, gdy kamera gracza go widzi,
- samo patrzenie na niego podnosi strach.

Gdzie pasuje:

- Habitable Zone,
- puste biura,
- koniec dlugiego jasnego korytarza.

## Pipe Runner

Scena:

- `Scenes/Enemies/PipeRunnerEnemy.tscn`

Skrypt:

- `Scripts/Enemies/PipeRunnerEnemy.cs`

Model:

- `Models/Backrooms/pipe_runner_low.obj`

Zachowanie:

- robi krotki windup,
- potem szarzuje prosto,
- po szarzy ma chwile slabosci.

Gdzie pasuje:

- Utility Halls,
- Electrical Station,
- waskie przejscia, gdzie gracz musi uzyc uniku albo FlashBurst.

## Office Mimic

Scena:

- `Scenes/Enemies/OfficeMimicEnemy.tscn`

Skrypt:

- `Scripts/Enemies/OfficeMimicEnemy.cs`

Model:

- `Models/Backrooms/office_mimic_chair.obj`

Zachowanie:

- udaje zwykly mebel,
- podnosi strach, gdy gracz jest blisko,
- zbliza sie, gdy gracz odwroci wzrok.

Gdzie pasuje:

- Abandoned Office,
- recepcje,
- pokoje socjalne,
- pomieszczenia, ktore wygladaja za normalnie.

## Corner Stalker

Scena:

- `Scenes/Enemies/CornerStalkerEnemy.tscn`

Skrypt:

- `Scripts/Enemies/CornerStalkerEnemy.cs`

Model:

- `Models/Backrooms/corner_stalker_heavy.obj`

Zachowanie:

- zaczyna ukryty,
- budzi sie przez `JumpscareTrigger`,
- idzie wolno, ale co kilka sekund robi zryw.

Gdzie pasuje:

- duzy jumpscare zza rogu,
- mini-boss poziomu,
- moment, gdzie gracz ma uciec zamiast walczyc do konca.

## Dodatkowe modele bez AI

Te modele sa przygotowane pod przyszle enemy:

- `Models/Backrooms/ceiling_crawler_placeholder.obj`
- `Models/Backrooms/false_exit_door_enemy.obj`

Pomysly:

- Ceiling Crawler: enemy na suficie, ktory spada po zgaszeniu swiatla.
- False Exit Door: drzwi, ktore wygladaja jak wyjscie, ale poruszaja sie, gdy gracz odwraca wzrok.

## Jak dodac enemy do levelu recznie

1. Otworz scene levelu.
2. Przeciagnij scene enemy z `Scenes/Enemies/`.
3. Ustaw pozycje na podlodze.
4. Ustaw eksportowane pola:
   - `AggroRange`,
   - `AttackDamage`,
   - `StartHidden`,
   - `StartInactive`.
5. Jesli enemy ma wyskoczyc zza rogu, dodaj `JumpscareTrigger` i ustaw `EnemyPath`.

## Jak dodac enemy do buildera

W `Scripts/World/BackroomsLevelBuilder.cs` sa gotowe metody:

```csharp
AddWatcherMannequin(position, "Nazwa");
AddPipeRunner(position, "Nazwa");
AddOfficeMimic(position, "Nazwa");
AddHiddenCornerStalker(enemyPosition, triggerPosition, "Tekst", 42f);
```

Najpierw dodawaj enemy oszczednie. W horrorze jeden dobrze ustawiony przeciwnik jest mocniejszy niz pieciu losowych.
