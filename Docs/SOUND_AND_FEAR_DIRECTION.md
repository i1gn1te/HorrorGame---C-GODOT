# Aranzacja dzwieku, strachu i klimatu

Inspiracje kierunkowe:

- Backrooms Wiki: poziomy, liminal spaces, katalogowanie dziwnych miejsc.
- From The Darkness: cisza, ciasnota, nasluchiwanie i poczucie bycia obserwowanym.

Nie kopiujemy konkretnych scen. Bierzemy lekcje projektowa: gracz ma bac sie przejsc przez zwykly korytarz, zanim pojawi sie przeciwnik.

## Filary strachu

1. Cisza przed informacja
   - Najpierw slychac normalny dzwiek techniczny.
   - Potem dzwiek robi sie minimalnie zly.
   - Dopiero pozniej cos dzieje sie w przestrzeni.

2. Ciemnosc jako obecnosc
   - Ciemnosc nie jest tylko brakiem swiatla.
   - Ciemnosc moze miec trigger, tekst, dzwiek i wzrost strachu.

3. Klaustrofobia
   - Zakrety, rury i niskie sufity sa mocniejsze niz otwarte areny.
   - Gracz powinien czuc, ze kamera jest blisko scian.

4. Falszywe bezpieczenstwo
   - Habitable Zone wyglada spokojniej, ale obrazy sugeruja, ze ktos obserwuje gracza.
   - Checkpoint nie jest magiczny. To tylko miejsce, ktore chwilowo pamieta gracza.

5. Oszczedne scare'y
   - Jumpscare ma byc rzadki.
   - Po jumpscare powinien pojawic sie gameplay: ucieczka, FlashBurst albo decyzja.

## Nowe pliki audio

Combat:

- `Assets/Audio/Combat/flash_burst.wav`
- `Assets/Audio/Combat/jumpscare_hit.wav`

Ambient:

- `Assets/Audio/Ambient/industrial_pipe_drone.wav`
- `Assets/Audio/Ambient/electric_station_loop.wav`
- `Assets/Audio/Ambient/office_silence_tone.wav`
- `Assets/Audio/Ambient/story_wall_whisper.wav`

Starsze pliki nadal dzialaja:

- `Assets/Audio/ambient_fluorescent_hum.wav`
- `Assets/Audio/ambient_low_rumble.wav`
- `Assets/Audio/stinger_dark_presence.wav`

## Skrypty audio

- `Scripts/Core/AudioMoodDirector.cs`
  - odpala petle audio,
  - zmienia threat level,
  - gra FlashBurst,
  - gra jumpscare,
  - gra szept obrazow.

- `Scripts/World/AudioProfileTrigger.cs`
  - ustawia profil audio poziomu przy spawnie.

- `Scripts/World/AudioBeatTrigger.cs`
  - podbija napiecie w konkretnym miejscu.

## Mapa audio poziomow

Level 0:

- Jarzeniowki i lekki rumble.
- Strach prawie niewidoczny.
- Obraz daje pierwszy szept.

Level 1:

- Wiekszy betonowy poglos.
- Niski rumble jest cichy.
- Miejsce wyglada bezpieczniej, ale brzmi za pusto.

Level 2:

- Industrialny dron rur.
- Brownout i miganie.
- Jumpscare ma osobny mocny hit.

Level 3:

- Elektryczny loop.
- Trzaski i niebieskie swiatlo.
- FlashBurst powinien brzmiec jak narzedzie desperacji, nie supermoc.

Level 4:

- Cichy ton biura.
- Okna odpalaja skok audio.
- Cisza ma byc bardziej straszna niz halas.

## Jak rozwijac dalej

1. Dodaj `FootstepPlayer` do gracza.
2. Zmieniaj dzwiek krokow po materiale podlogi.
3. Dodaj oddech gracza przy wysokim strachu.
4. Dodaj cichy dzwiek enemy, ktory psuje jarzeniowki.
5. Dodaj trigger absolutnej ciszy na 2 sekundy przed duzym scare.
