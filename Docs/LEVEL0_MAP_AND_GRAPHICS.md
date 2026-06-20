# Level 0 / Threshold - grafiki i mapa

Ten folder zawiera oryginalne grafiki pomocnicze do pierwszego poziomu inspirowanego liminal spaces / Backrooms:

- `Assets/Graphics/Level0/level0_topdown_map.svg` - mapa top-down do budowy blockoutu.
- `Assets/Graphics/Level0/level0_topdown_map.png` - rasterowy podglad mapy.
- `Assets/Graphics/Level0/level0_mood_sheet.svg` - arkusz kierunku wizualnego.
- `Assets/Graphics/Level0/level0_mood_sheet.png` - rasterowy podglad mood sheetu.
- `Assets/Graphics/Level0/level0_wallpaper_tile.svg` - placeholder kafla tapety.
- `Assets/Graphics/Level0/level0_carpet_tile.svg` - placeholder kafla mokrego dywanu.
- `Assets/Graphics/Level0/level0_ceiling_tile.svg` - placeholder kafla sufitu.
- `Assets/Graphics/Level0/level0_minimap_mock.svg` - uproszczona grafika mapy/minimapy.

## Dane klimatu

Pierwszy poziom traktujemy jako Threshold / Level 0: monotonne zolto-bezowe pomieszczenia biurowe, wilgotny dywan, powtarzalne sciany, lampy jarzeniowe, brak okien, nielogiczny uklad i rzadkie anomalie. To ma byc poczatek gry, wiec strach jest jeszcze cichy: bardziej niepokoj niz otwarta walka.

## Co z tego budujemy w Godot

1. Start w jasniejszym, z pozoru pustym obszarze.
2. Dlugie korytarze i kilka slepych odnog.
3. Boczny pokoj z kluczem.
4. Pierwszy przeciwnik nie od razu przy graczu, tylko za zakretem.
5. Blackout zone blisko wyjscia.
6. Czerwony pokoj jako mala zapowiedz, ze pozniejsze levele beda bardziej agresywne.

## Jak uzywac

Otworz `level0_topdown_map.svg` w przegladarce albo programie graficznym i przepisuj uklad na CSGBox3D w Godot. Kafle SVG mozesz zaimportowac jako tekstury tymczasowe, ale docelowo najlepiej zrobic ich wersje w Blenderze/Substance/Krita.

## Zastosowanie w grze

Grafiki sa juz podlaczone do poziomow:

- `M_Wall.tres` uzywa `level0_wallpaper_tile.svg`.
- `M_Floor.tres` uzywa `level0_carpet_tile.svg`.
- `M_Ceiling.tres` uzywa `level0_ceiling_tile.svg`.
- `Level01_LiminalLobby.tscn` ma plansze `MapPoster` i `MoodPoster`.
- `Level02_ServiceCorridor.tscn` ma plansze `DamagedMapPoster` i `MoodPoster`.

Poniewaz oba poziomy korzystaja z materialow `M_Wall`, `M_Floor` i `M_Ceiling`, tekstury Level 0 pojawiaja sie na kazdym obecnym levelu.

## Attribution

Kierunek jest inspirowany opisami Level 0 / Threshold z The Backrooms Wiki. Grafiki w tym projekcie sa oryginalnymi assetami pomocniczymi i nie kopiuja obrazow ani tekstow ze zrodla.
