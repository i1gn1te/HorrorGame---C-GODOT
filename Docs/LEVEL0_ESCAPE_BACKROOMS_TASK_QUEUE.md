# Level 0 - kolejka zadan na haslo "dawaj robimy"

Ten plik jest kolejka prac do odpalenia jutro, kiedy napiszesz:

```text
dawaj robimy
```

Cel: przebudowac Level 0 tak, zeby mocniej przypominal opis z Escape The Backrooms Wiki, ale nadal byl nasza wlasna implementacja w Godot.

Zrodlo inspiracji:

- https://escapethebackrooms.fandom.com/wiki/Level_0#Main_Level

## Co bierzemy ze strony

Wyglad:

- Level 0 ma przypominac zaplecze/biuro z okolic lat 70.
- Sciany: jasne zolte tapety.
- Podloga: wilgotna, zaniedbana wykladzina.
- Swiatlo: jarzeniowki rozmieszczone nierowno.
- Pokoje maja wygladac podobnie, ale nie identycznie.
- Level ma miec dwie warstwy: glowna jasniejsza i druga ciemniejsza po upadku.

Kolejnosc gameplayu:

- start nowej gry,
- spawn w pokoju z filarami albo obok niego,
- notatka blisko spawnu,
- szukanie pokoju z zepsuta drabina,
- zebranie 4 czesci drabiny,
- naprawa drabiny,
- zdobycie klucza na gorze,
- odblokowanie drewnianych drzwi,
- przejscie przez Pitfalls,
- wyjscie wentylacja do Level 1.

Alternatywna droga:

- upadek w Pitfalls nie konczy gry,
- gracz trafia do drugiej, ciemniejszej warstwy,
- strzalki na scianach prowadza do domopodobnej strefy,
- w tej strefie jest kuchnia z niebieskim swiatlem, ciemny pokoj, latarka, schody i wentylacja do Level 1.

Obiekty do przygotowania:

- notatka,
- zepsuta drabina,
- 4 czesci drabiny,
- klucz,
- drewniane drzwi,
- Pitfalls,
- lina jako element scenografii przy wyjsciu,
- vent exit,
- latarka,
- jeden prototyp entity w klimacie umiarkowanego zagrozenia.

Zakres na pierwszy sprint:

- glowny Level 0,
- start nowej gry,
- pokoje z filarami,
- notatka przy spawnie,
- zepsuta drabina,
- 4 czesci drabiny,
- klucz na gorze po naprawie drabiny,
- drewniane drzwi,
- Pitfalls,
- ciemniejsza druga warstwa jako opcjonalna droga po upadku,
- wyjscie przez vent do Level 1.

Nie robimy jeszcze:

- pelnej mniejszej strefy z wejscia od Level 8,
- Manilla Room,
- chainsaw/locker code,
- Hub Door,
- Level 0.11.

## 0. Zasady techniczne

- Nie niszczyc obecnego ruchu gracza.
- Nie ruszac flow pozniejszych leveli bez potrzeby.
- Zostawic `Level02_HabitableZone.tscn` jako nastepny level.
- Nie kopiowac doslownie tekstow ze strony.
- Budowac modularnie, z prostych bryl i tekstur, zeby pozniej podmieniac na modele z Blendera.

## 1. Research lock

Status: pending

Zadania:

- Zrobic krotka notatke z cech wygladu Level 0.
- Rozdzielic elementy na:
  - wyglad,
  - layout,
  - obiekty,
  - gameplay,
  - audio,
  - przejscia.
- Ustalony target: biuro/retail back rooms z lat 70., jasne zolte tapety, mokrawa zaniedbana wykladzina, losowo rozmieszczone jarzeniowki, pokoje podobne, ale nie identyczne.

Done kiedy:

- Wiemy, co implementujemy w pierwszym podejsciu, a co zostaje na pozniej.

## 2. Backup obecnego Level 0

Status: pending

Zadania:

- Zachowac obecny `Level01_LiminalLobby.tscn` jako punkt odniesienia.
- Jesli trzeba, utworzyc scene backupowa:

```text
Scenes/Levels/Archive/Level01_LiminalLobby_BeforeEscapeStyle.tscn
```

Done kiedy:

- Mozna bezpiecznie przebudowac Level 0 bez utraty dotychczasowego prototypu.

## 3. Podstawowy layout Main Level

Status: pending

Zadania:

- Zrobic Level 0 jako nieliniowy uklad podobnych pomieszczen.
- Dodac glowny pokoj z filarami.
- Dodac alternatywny spawn: obok pokoju z filarami.
- Dodac kilka przejsc, ktore wygladaja podobnie, ale maja inne proporcje.
- Dodac srednia skale poziomu: nie za ogromny, ale wiekszy niz obecny lobby blockout.
- Zachowac wrazenie, ze pokoje sa podobne, lecz nie sa identyczne.

Elementy wizualne:

- zolte tapety na scianach,
- wilgotna brudna wykladzina,
- sufit z jarzeniowkami,
- nieregularne swiatla,
- filary,
- puste retail/back-office przejscia.

Done kiedy:

- Gracz moze przejsc przez kilka pomieszczen i poczuc, ze to labirynt Level 0.

## 4. Tapety i podloga

Status: pending

Zadania:

- Uzyc obecnych wariantow tapet Level 0.
- Utrzymac losowe wzory na scianach przez `RandomWallPatternApplier`.
- Sprawdzic, czy nowy dywan `M_Floor.tres` wyglada jak wilgotna wykladzina, a nie kamien.
- Dodac lokalne plamy/brud jako decals albo osobne plaskie meshe na podlodze.

Done kiedy:

- Podloga i sciany od razu mowia "Backrooms Level 0".

## 5. Swiatlo jarzeniowe

Status: pending

Zadania:

- Rozmiescic swiatla nieregularnie.
- Czesc lamp ma byc jasna.
- Czesc lamp ma migac.
- Czesc lamp ma byc martwa albo przygaszona.
- Zostawic pierwszy poziom generalnie jasny, ale z pojedynczymi niepokojacymi ciemnymi fragmentami.

Done kiedy:

- Level jest jasny, ale nie komfortowy.

## 6. Start nowej gry

Status: pending

Zadania:

- Zrobic prosty start bez dlugiej cutscenki:
  - gracz wchodzi do backroomu,
  - po chwili orientuje sie, ze droga powrotna znikla.
- Dodac komunikat HUD zamiast pelnej sceny filmowej.
- Dodac drzwi lub ciemne wejscie za graczem, ktore po chwili przestaje prowadzic gdziekolwiek.

Done kiedy:

- Gracz rozumie: "wszedlem, nie moge wyjsc ta sama droga".

## 7. Notatka przy spawnie

Status: pending

Zadania:

- Dodac obiekt notatki na scianie blisko obu mozliwych spawn pointow.
- Notatka ma byc naszym tekstem, nie kopia strony.
- Notatka sugeruje:
  - znajdz drabine,
  - nie ufaj identycznym pokojom,
  - swiatlo nie zawsze pokazuje prawde.

Mozliwa implementacja:

- `WallStoryTrigger`,
- plakat/notatka jako `MeshInstance3D` z tekstura,
- HUD message po podejsciu.

Done kiedy:

- Przy starcie gracz ma pierwszy delikatny cel.

## 8. Pokoj ze zepsuta drabina

Status: pending

Zadania:

- Stworzyc osobny pokoj z uszkodzona drabina.
- Drabina ma byc widoczna i rozpoznawalna.
- Dodac brakujace miejsca na 4 czesci.
- Dodac komunikat: drabina jest niekompletna.

Assety:

- prosty model drabiny z klockow albo OBJ,
- 4 pickupy czesci drabiny,
- docelowo model z Blendera.

Done kiedy:

- Gracz znajduje drabine i wie, ze musi znalezc 4 elementy.

## 9. Cztery czesci drabiny

Status: pending

Zadania:

- Dodac 4 pickupy:
  - `LadderPiece_01`,
  - `LadderPiece_02`,
  - `LadderPiece_03`,
  - `LadderPiece_04`.
- Rozrzucic je po roznych pokojach Level 0.
- Minimum jedna czesc powinna byc w pokoju z filarami.
- Minimum jedna czesc powinna byc przy ciemniejszym, martwym rogu.
- Minimum jedna czesc powinna wymagac obejscia przez podobne pokoje.

Skrypt do stworzenia:

```text
Scripts/World/LadderPiecePickup.cs
Scripts/World/RepairableLadder.cs
```

Done kiedy:

- Gracz moze zebrac 4 czesci i naprawic drabine.

## 10. Naprawa drabiny i klucz

Status: pending

Zadania:

- Po zebraniu 4 czesci drabina zmienia stan na naprawiona.
- Gracz moze wejsc/aktywowac drabine.
- Zamiast pelnej animacji na start:
  - trigger przenosi gracza na mala gorna platforme,
  - albo pokazuje krotki fade/komunikat i daje klucz.
- Dodac maly box/platforme na gorze z kluczem.

Done kiedy:

- Naprawiona drabina daje graczowi klucz do drewnianych drzwi.

## 11. Drewniane drzwi do Pitfalls

Status: pending

Zadania:

- Dodac drewniane drzwi po lewej stronie od pokoju drabiny.
- Drzwi sa zamkniete do momentu zdobycia klucza.
- Po uzyciu klucza przenosza gracza do strefy Pitfalls.

Mozna uzyc:

- obecnego `LockedDoor.cs`,
- albo nowej wersji tylko pod ladder key.

Done kiedy:

- Klucz ma jasne zastosowanie.

## 12. Pitfalls - glowna droga

Status: pending

Zadania:

- Stworzyc osobna sekcje poziomu: inny pokoj niz reszta Level 0.
- Dodac waskie przejscia/kladki.
- Dodac dziury/pity.
- Na drugim koncu dac rope/vent/exit point.
- Nie robic jeszcze pelnego Q/E balancing systemu, jesli to za duzy zakres.
- Pierwsza wersja:
  - waskie platformy,
  - kolizje,
  - trigger upadku,
  - wyjscie przez vent.

Done kiedy:

- Gracz moze przejsc przez Pitfalls do vent exit.

## 13. Upadek do drugiej warstwy

Status: pending

Zadania:

- Gdy gracz spadnie w Pitfalls, nie zabijac go od razu.
- Przeniesc go do drugiej warstwy Level 0.
- Druga warstwa:
  - ciemniejsza,
  - bardziej otwarta,
  - podobna do Level 0, ale mniej komfortowa.
- Dodac narysowane strzalki na scianach.
- Strzalki prowadza do alternatywnego wyjscia.

Done kiedy:

- Upadek staje sie osobna droga, nie tylko porazka.

## 14. Alternatywna droga: domopodobna strefa

Status: pending

Zadania:

- Po strzalkach gracz trafia do pokoju, ktory wyglada inaczej niz reszta poziomu.
- Zrobic domopodobna mini-strefe:
  - kuchnia z niebieskim swiatlem,
  - ciemny pokoj,
  - szuflada/drawer z latarka,
  - schody,
  - korytarz,
  - vent do Level 1.

Pierwsza wersja moze byc klockowa.

Done kiedy:

- Druga warstwa ma sensowny koniec i klimat "to nie powinno byc tutaj".

## 15. Latarka

Status: pending

Zadania:

- Dodac pickup latarki w ciemnym pokoju.
- Jesli nie robimy jeszcze prawdziwej latarki:
  - podniesienie latarki podbija lokalne swiatlo,
  - albo wlacza `SpotLight3D` przy kamerze.

Skrypt do stworzenia:

```text
Scripts/Player/PlayerFlashlight.cs
Scripts/World/FlashlightPickup.cs
```

Done kiedy:

- Ciemna druga warstwa ma mechaniczny powod istnienia.

## 16. Entity risk: Bacteria jako prototyp

Status: pending

Zadania:

- Dodac jednego enemy inspirowanego rola Bacteria, ale nie kopiowac wygladu 1:1.
- Umiescic go ostroznie, raczej w drugiej warstwie albo blisko Pitfalls.
- Pierwsza wersja:
  - wysoka sylwetka,
  - dzwiek obecnosci,
  - wolne sciganie,
  - FlashBurst daje tylko chwile.

Mozna rozwinac z:

```text
CornerStalkerEnemy
WatcherMannequinEnemy
```

Done kiedy:

- Level 0 ma umiarkowane ryzyko entity, ale nadal nie jest arena walki.

## 17. Audio Level 0

Status: pending

Zadania:

- Jarzeniowki jako glowna warstwa.
- Mokra wykladzina: ciche kroki/tlumienie.
- Dalszy pusty poglos.
- Pitfalls: nizszy ton i wiecej przestrzeni.
- Druga warstwa: mniej lamp, wiecej basu.
- Domopodobna strefa: niebieskie buczenie i cisza.

Done kiedy:

- Audio zmienia sie miedzy glownym Level 0, Pitfalls i druga warstwa.

## 18. Exits

Status: pending

Zadania:

- Glowna droga:
  - Pitfalls,
  - vent po drugiej stronie,
  - przejscie do `Level02_HabitableZone.tscn`.
- Alternatywna droga:
  - druga warstwa,
  - strzalki,
  - domopodobna strefa,
  - vent na koncu,
  - przejscie do `Level02_HabitableZone.tscn`.

Done kiedy:

- Obie drogi koncza sie przejsciem do Level 1/Level02.

## 19. Testy po implementacji

Status: pending

Zadania:

- `dotnet build`
- Godot headless `--build-solutions --quit`
- Start `Scenes/Main.tscn`
- Test:
  - spawn,
  - notatka,
  - znalezienie drabiny,
  - 4 czesci,
  - naprawa drabiny,
  - klucz,
  - drzwi,
  - Pitfalls,
  - upadek do drugiej warstwy,
  - alternatywne wyjscie,
  - przejscie do Level02.

Done kiedy:

- Gracz moze przejsc caly Level 0 przynajmniej jedna droga.

## 20. Priorytet jutro

Jesli jutro napiszesz "dawaj robimy", zaczynamy od:

1. Backup sceny.
2. Przebudowa layoutu Main Level.
3. Pokoj z filarami i notatka.
4. Pokoj z drabina.
5. 4 czesci drabiny.
6. Naprawa drabiny i klucz.
7. Drewniane drzwi do Pitfalls.
8. Minimalne Pitfalls.

Potem, jesli starczy czasu:

9. Druga warstwa po upadku.
10. Strzalki.
11. Domopodobna strefa.
12. Latarka.
13. Entity.
