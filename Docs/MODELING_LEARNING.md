# Nauka modelowania 3D pod Gra3D

Ten plik jest sciezka nauki Blendera pod ten konkretny projekt. Nie zaczynaj od idealnej postaci. Najpierw rob rzeczy, ktore buduja poziom: sciany, drzwi, rury, panele, lampy, ramki obrazow i proste sylwetki enemy.

## Zasada projektu

Najpierw blockout w Godot, potem prosty model w Blenderze, potem podmiana placeholdera. Jesli cos nie dziala jako klocek, ladny model tego nie uratuje.

## Nowe modele przykladowe

Folder:

- `Models/Backrooms/`

Modele:

- `large_corner_stalker.obj` - duzy klocek-enemy do jumpscare zza rogu.
- `flash_emitter.obj` - prosty przedmiot pod mechanike FlashBurst.
- `pipe_valve.obj` - zawor do Utility Halls.
- `electrical_panel.obj` - panel do Electrical Station.
- `office_window_trap.obj` - czarne okno do Abandoned Office.
- `story_frame.obj` - ramka pod obrazy fabularne.
- `watcher_mannequin_tall.obj` - manekin, ktory rusza gdy gracz nie patrzy.
- `pipe_runner_low.obj` - niski szybki enemy do rur.
- `office_mimic_chair.obj` - krzeslo/mimic do biura.
- `corner_stalker_heavy.obj` - duzy enemy do jumpscare zza rogu.
- `ceiling_crawler_placeholder.obj` - model pod przyszlego enemy sufitowego.
- `false_exit_door_enemy.obj` - model pod falszywe drzwi-enemy.
- `backrooms_assets.mtl` - materialy OBJ.

Starsze modele nadal sa w:

- `Models/Examples/liminal_locked_door.obj`
- `Models/Examples/crooked_fluorescent_lamp.obj`
- `Models/Examples/watcher_mannequin.obj`
- `Models/Examples/service_cabinet.obj`
- `Models/Examples/cold_key.obj`

## Jak uczyc sie na tych modelach

1. Otworz `large_corner_stalker.obj` w Blenderze.
2. Zobacz, ze to tylko bryly i sylwetka.
3. Dodaj bevel na krawedziach.
4. Wydluz rece albo glowe, ale nie przesadzaj z detalem.
5. Eksportuj jako `.glb` do `Models/Backrooms/`.
6. W Godot podmien klocek enemy na nowy model.

## Plan nauki

### Tydzien 1: bryly i skala

- Cube, move, rotate, scale.
- Snap do siatki.
- Cel: zrob 5 wariantow sciany i 5 wariantow drzwi.

### Tydzien 2: propsy liminalne

- Lampy jarzeniowe.
- Szafki techniczne.
- Panele scienne.
- Tabliczki ewakuacyjne.
- Cel: kazdy obiekt ma czytelna sylwetke z daleka.

### Tydzien 3: modularnosc

- Sciana 2 m.
- Rog.
- Sufit.
- Podloga.
- Drzwi.
- Wascy korytarz.
- Cel: z 10 modeli ukladasz caly poziom.

### Tydzien 4: pierwszy enemy

- Nie zaczynaj od anatomii.
- Zrob potwora z pudel.
- Najpierw sylwetka, potem detale.
- Cel: enemy jest rozpoznawalny nawet bez tekstury.

### Tydzien 5: UV i proste tekstury

- Naucz sie unwrap.
- Zrob material betonu, metalu, czarnego szkla i papieru.
- Cel: jedna tekstura sciany wyglada dobrze powtorzona wiele razy.

### Tydzien 6: eksport do Godot

- Eksportuj jako `.glb`.
- Uzywaj skali metrycznej.
- Zeruj transformacje w Blenderze.
- Cel: model ma poprawny rozmiar obok gracza.

## Minimalne zasady eksportu

- Gracz ma okolo 1.8 m.
- Drzwi powinny miec okolo 2.1-2.3 m wysokosci.
- Trzymaj malo materialow na jeden model.
- Nazwy obiektow maja mowic, czym sa, np. `pipe_valve_A`, `window_black_trap_A`.
- `.blend` zostaw jako plik roboczy, do gry dawaj `.glb`.

## Zadania praktyczne

1. Otworz `story_frame.obj` i dodaj cienka szybe.
2. Otworz `pipe_valve.obj` i zrob druga wersje z urwana raczka.
3. Otworz `electrical_panel.obj` i dodaj 3 przelaczniki.
4. Otworz `large_corner_stalker.obj` i zrob wersje z za dlugimi rekami.
5. Zrob wlasny `office_chair_broken.glb` i ustaw go w Level 4.

## Horror w modelach

Nie kazdy obiekt ma byc straszny. Wiekszosc powinna byc zwyczajna. Strach robi jeden detal nie na miejscu:

- lampa wisi za nisko,
- korytarz jest minimalnie za waski,
- okno jest czarne mimo swiatla,
- panel elektryczny ma slady paznokci,
- ramka pokazuje zdjecie pokoju, w ktorym gracz jeszcze nie byl.
