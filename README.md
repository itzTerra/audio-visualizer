# PV178-Project - Audio Visualizer

**Opravujícímu se dopředu omlouvám za případné problémy při kompilaci. Knihovna SoundFlow je velmi nová a např. na Linuxu jsem musel zkopírovat celý podadresář `runtimes/linux-x64/native/libminiaudio.so` z `bin/Debug/net9.0` do kořenové adresáře projektu, aby mi to fungovalo.**

## Used libs

- [Avalonia](https://docs.avaloniaui.net/docs/basics/user-interface/controls/builtin-controls)
- [Ursa](https://github.com/irihitech/Ursa.Avalonia) [(Demo)](https://irihitech.github.io/Ursa.Avalonia/)
  - [Semi Theme](https://docs.irihi.tech/semi/en/docs/)
- [Icons.Avalonia](https://github.com/Projektanker/Icons.Avalonia)
- [SoundFlow](https://github.com/LSXPrime/SoundFlow)

## Known Issues
- Video export unsupported
- Theme module forms are not validated against nonsense inputs
- Theme Explorer double-click to select works only on the text part of a node
- Theme Explorer doesn't show a drop location indicator when hovering over an edge of a category (style issue)
- App displays playing sound icon in the taskbar all the time, even when not playing sound
- Theme module groups are not used in UI yet


## Assignment

**2 Jaké jsou požadavky na vaši aplikaci (co bude umět/podporovat)? Rozsah musí být podobný ukázkovým tématům v interaktivní osnově.**

- Mohu načíst soubor z podporovaných audio formátů.
- Mohu načtené audio přehrávat, nastavit hlasitost, posouvat se na časové ose.
- Hlavní část okna vyplňuje display s grafickým výstupem aktuálně vybraného bodu na časové ose (přehrávání audia přehrává i video).
- Mohu vybrat Theme grafického výstupu.

    Každý Theme má své nastavitelné parametry (intenzita, barvy, toggle efektů…).

    Přednastavený Theme (s konkrétními upravenými parametry) lze “Uložit jako” Custom Theme.

    Custom Themes lze dále klonovat, procházet, upravovat, mazat.

    V Themes lze vyhledávat.

    Mohu vytvářet, přejmenovávat, a mazat kategorie, do kterých mohu zařadit nějaký Theme.

- Mohu exportovat aktuální obrázek na displeji do zvoleného adresáře (FileInput).
- Mohu exportovat výsledné video do zvoleného adresáře (FileInput).

    V okně exportu mohu vybrat, zda-li chci ve videu původní audio či ne (Checkbox).

- Mohu přejít do globálních (systémových) nastavení aplikace a nastavit:
    - výchozí složku exportovaných souborů
    - light/dark theme

***Rozšíření do budoucna***

- **můj main goal: theme, který používá jako zdroj informace k vykreslování videa AI embedding vektory audia (např. z vggish)**
- projektový režim — v aplikaci lze otevřít adresář, který bude pak fungovat jako projekt

    výstupy se budou ukládat do projektové složky, výchozí adresář pro FileInput vstupů bude taky projektová složka

    možnost uložit Theme pouze lokálně v projektu a ne systémově

- vytváření vlastních themes s vlastními parametry a s nimi spojenými metodami vykreslování
- možnost exportu jen vybrané části časové osy
- možnost na různé části audia namapovat jiný theme, nebo na stejnou část mít více grafických výstupů (layers + opacity could be nice)
- nahrání a práce s více audio stopami

**3 Jakým způsobem využijete soubory nebo databázi?**

Plánuji ukládat data do .json souborů.

Import audia a export audia i videa.

**4 Jakým způsobem využijete Tasks / asynchronní metody?**

Import a export audia, videa.

Ukládání .json konfigurací.

Zobrazování videa, přehrávání zvuku.

(Celé je to asynchronní…)
