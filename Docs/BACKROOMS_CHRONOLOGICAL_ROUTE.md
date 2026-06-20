# Chronologiczna trasa Backrooms w prototypie

Ten dokument opisuje trase gry od wejscia do Backrooms. Korzystamy z Backrooms Wiki jako inspiracji klimatu i kolejnosci, ale nie kopiujemy ich tekstow, potworow ani gotowych scen.

## Zrodla inspiracji

- Level 0 - Threshold: https://backrooms-wiki.wikidot.com/level-0
- Level 1 - Habitable Zone: https://backrooms-wiki.wikidot.com/level-1
- Level 2 - Abandoned Utility Halls: https://backrooms-wiki.wikidot.com/level-2
- Level 3 - Electrical Station: https://backrooms-wiki.wikidot.com/level-3
- Level 4 - Abandoned Office: https://backrooms-wiki.wikidot.com/level-4

## Trasa w grze

1. `Scenes/Levels/Level01_LiminalLobby.tscn`
   - Adaptacja Level 0.
   - Jasny zolty/pusty start, duzo wolnej przestrzeni, brak enemy.
   - Strach buduje cisza, lampy i pierwsze obrazy na scianach.

2. `Scenes/Levels/Level02_HabitableZone.tscn`
   - Adaptacja Level 1.
   - Duza betonowa przestrzen, slupy, slady ludzi, dalej bez mocnego starcia.
   - Gracz ma poczuc, ze "bezpieczne" miejsce wcale nie jest przyjazne.

3. `Scenes/Levels/Level03_UtilityHalls.tscn`
   - Adaptacja Level 2.
   - Ciasne korytarze, rury, brazowawe swiatlo, martwe zakrety.
   - Pierwszy duzy jumpscare zza rogu i realne uzycie FlashBurst.

4. `Scenes/Levels/Level04_ElectricalStation.tscn`
   - Adaptacja Level 3.
   - Elektryczne panele, niebieskie miganie, glosny dron.
   - Gracz dostaje bardziej aktywna presje i przeciwnika do zatrzymania blyskiem.

5. `Scenes/Levels/Level05_AbandonedOffice.tscn`
   - Adaptacja Level 4.
   - Puste biuro, boksy, czarne okna, cisza zamiast chaosu.
   - Okna dzialaja jak pulapki strachu i fragment fabuly.

## Narastanie grozy

Poziomy nie maja byc coraz bardziej "ciemne" w prosty sposob. Maja byc coraz mniej normalne:

- Level 0: wszystko wyglada zwyczajnie, ale jest za puste.
- Level 1: widac slady ludzi, ale to wcale nie uspokaja.
- Level 2: przestrzen robi sie ciasna i atakuje zza martwych katow.
- Level 3: swiatlo i dzwiek staja sie mechaniczna agresja.
- Level 4: znow jest ciszej, ale obrazy i okna wiedza za duzo.

## Jak dodac kolejny level

1. Stworz scene w `Scenes/Levels/`.
2. Dodaj do niej `Scripts/World/BackroomsLevelBuilder.cs`.
3. Ustaw `BackroomsLevel` albo dopisz nowy profil w builderze.
4. W poprzedniej scenie ustaw `NextScenePath` na nowa scene.
5. Dodaj mape SVG, plakat fabularny i profil audio.
