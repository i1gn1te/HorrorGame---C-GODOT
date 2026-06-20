# Walka, strach i jumpscare

Gra idzie w strone liniowego first-person horroru akcji. Nie robimy zwyklego puzzlera. Gracz ma miec narzedzie do reakcji, ale nadal ma czuc, ze miejsce jest silniejsze od niego.

## FlashBurst

Skrypt: `Scripts/Player/FlashBurstAbility.cs`

FlashBurst to obecna mechanika walki. Dziala jak krotki impuls swiatla:

- `LPM` odpala szybszy, tanszy blysk.
- `PPM` odpala mocniejszy blysk.
- Blysk trafia tylko przeciwnikow przed kamera, w stozku.
- Trafiony enemy dostaje obrazenia i krotki stun.
- Blysk kosztuje stamina, wiec nie da sie spamowac bez konsekwencji.
- Blysk lekko obniza strach, bo gracz odzyskuje kontrole na moment.

Najwazniejsze pola:

```csharp
[Export] public float Range = 7.5f;
[Export] public float ConeAngleDegrees = 38f;
[Export] public float EnemyStunSeconds = 1.45f;
```

## Strach gracza

Skrypt: `Scripts/Player/PlayerFear.cs`

Strach jest osobnym paskiem na HUD. Na razie nie zabija, ale juz daje podstawe pod:

- drzenie kamery,
- halucynacje,
- zmiane audio,
- falszywe komunikaty,
- slabsza regeneracje staminy.

Trigger strachu:

- `Scripts/World/FearZoneTrigger.cs`

Uzycie: wrzucasz Area3D w ciemny fragment korytarza, ustawiasz `FearAmount` i opcjonalny tekst.

## Jumpscare

Skrypt: `Scripts/World/JumpscareTrigger.cs`

Jumpscare nie jest tylko obrazkiem. Robi kilka rzeczy naraz:

- aktywuje ukrytego przeciwnika,
- odpala mocniejszy dzwiek,
- podbija threat level audio,
- dodaje strach,
- pokazuje krotki komunikat.

Najlepsze uzycie:

1. Ustaw enemy za rogiem.
2. Daj mu `StartHidden = true` i `StartInactive = true`.
3. Postaw `JumpscareTrigger` przed zakretem.
4. Ustaw `EnemyPath` na enemy.
5. Nie naduzywaj tego. Jeden mocny jumpscare po dlugiej ciszy dziala lepiej niz losowy halas co minute.

## Obrazy fabularne

Skrypt: `Scripts/World/WallStoryTrigger.cs`

Obraz na scianie moze:

- pokazac fragment historii,
- dodac strach,
- odpalic cichy szept,
- podbic audio threat.

To daje fabule bez cutscenek. Gracz rozumie swiat przez sciany, mapy, plakaty i okna.

## Okna pulapki

Skrypt: `Scripts/World/WindowTrapTrigger.cs`

Uzywany w Level 4. Czarne okno nie jest dekoracja. Gdy gracz podejdzie za blisko:

- strach rosnie,
- gra odpala jumpscare sound,
- HUD pokazuje dziwny tekst.

## Audio profilu levelu

Skrypt: `Scripts/World/AudioProfileTrigger.cs`

Kazdy level moze zmienic ambient przez trigger przy spawnie. Uzywa `AudioMoodDirector`.

Przyklady nowych plikow audio:

- `Assets/Audio/Combat/flash_burst.wav`
- `Assets/Audio/Combat/jumpscare_hit.wav`
- `Assets/Audio/Ambient/industrial_pipe_drone.wav`
- `Assets/Audio/Ambient/electric_station_loop.wav`
- `Assets/Audio/Ambient/office_silence_tone.wav`
- `Assets/Audio/Ambient/story_wall_whisper.wav`
