# MKON
# 🏎️ Mario Kart Ohne Nintendo

> „Ein emotionaler und mitreißender Fun-Racer, der euch zur absoluten Verzweiflung bringen wird."

---

## 📋 Projektübersicht

| | |
|---|---|
| **Genre** | Fun Racer / Arcade |
| **Engine** | Unity 6000.3.5f2 |
| **Plattform** | Windows (aktuelles Windows-System) |
| **Zielgruppe** | Jugendliche ab 16 Jahre & junge Erwachsene |

| **USK** | 16 |
| **Entwicklungszeit** | 21.01.2026 – 27.02.2026 (~5 Wochen) |
| **Teamgröße** | 2 Personen |
| **Abgabedatum** | 27.02.2026 |

---

## 🎮 Spielbeschreibung

**Mario Kart Ohne Nintendo** ist ein schneller, chaotischer Arcade-Fun-Racer mit Drift-basiertem Fahrsystem, taktischem Item-Einsatz und lokalem Multiplayer. Inspiriert von Mario Kart, Crash Team Racing und Sonic Racing liegt der Fokus auf einem sauberen, mitreißenden Gameplay-Loop – „easy to learn, hard to master".

Drei einzigartige Strecken in Fantasiewelten, sechs spielbare Charaktere mit eigenen Persönlichkeiten und ein eskalierendes Item-System sorgen für spannende Rennen bis zur letzten Kurve.

---

## 🕹️ Steuerung

### Ingame

| Aktion | Tastatur | Controller |
|---|---|---|
| Lenken | `A` / `D` oder `← →` | Left Stick |
| Gas | `W` / `↑` | `A` |
| Bremsen | `S` / `↓` | `B` |
| Item benutzen | `E` | Left Trigger |
| Drift | `Shift` | Right Trigger |
| Reset | `T` | D-Pad Left |

### Menü

Navigation und Auswahl erfolgen ausschließlich per **Mausklick**.

---

## ✅ Features

- [x] 3 spielbare Strecken (Desert City, Schneewelt, Dungeon)
- [x] 6 spielbare Charaktere / Karts
- [x] Vollständiger Gameplay-Loop: Start → Kart wählen → Strecke wählen → Rennen → Endscreen
- [x] KI-Gegner mit Waypoint-Navigation
- [x] Item-System mit 6 Items (Fallen, Buffs, Projektile)
- [x] Spielmodus: **Normal** (3 Runden, Bestzeiten)
- [x] Spielmodus: **Last Out** (letzter Spieler wird eliminiert, sobald der erste ins Ziel fährt)
- [x] Geschwindigkeitsmultiplikator für hintere Plätze (Rubberbanding)
- [x] 17 Musik-Tracks (zufällig abgespielt)
- [x] Soundeffekte (Motor, Drift, Kollision, Items, ...)
- [x] Spielstatistiken über HTTP / JSON gespeichert
- [x] MiniMap
- [x] Animationen

---

## 🧑‍🤝‍🧑 Charaktere

> 💡 **Für Entwickler:** Neue Charaktere können einfach ergänzt werden – einfach ein neues Charakter-Prefab anlegen und in der Kart-Auswahl registrieren.

| Charakter | Rolle |
|---|---|
| 🟠 **Oobi** | Hektischer Sprinter – maximale Beschleunigung, bricht leicht aus |
| 🔵 **Oodi** | Relaxter Cruiser – stabiles Handling, driftet wie auf Schienen |
| 🟢 **Ooli** | Technik-Genie – niedriger Top-Speed, Items wirken länger |
| 🔴 **Oopi** | Schwerer Tank – riesiger Radius, rammt Gegner von der Piste |
| 🟣 **Oozi** | Chaotischer Trickser – unberechenbar, extreme Item-Kontrolle |
| 💻 **Dennis** | Taktischer Entwickler – ausgeglichene Werte, schnellere Reaktion nach Treffern |

---

## 🗺️ Strecken

- **Desert City** – Mittelalterliche Wüstenstadt
- **Schneewelt** – Eisige Winterlandschaft
- **Dungeon** – Finsterer Indoor-Dungeon

---

## 🍕 Item-System

Items werden durch das Durchfahren einer **Pizza** aufgesammelt. Der Spieler kann bis zu **4 Items** speichern und nutzt sie in Einsammelreihenfolge. Die Item-Wahrscheinlichkeit orientiert sich am aktuellen Ranking (vorne eher Fallen, hinten eher Projektile).

> 💡 **Für Entwickler:** Neue Items können einfach ergänzt werden – neues Item-Script erstellen, in den Item-Pool eintragen und Kategorie (Falle / Buff / Projektil) zuweisen.

| Kategorie | Items |
|---|---|
| **Fallen** | Pfeffer, Holy Bomb |
| **Buffs** | Schild, Speed-Boost |
| **Projektile** | Kuchen, Kokosnuss |

---

## 🗄️ Datenbank & API

Spielstatistiken werden über **HTTP + JSON** gespeichert (CRUD-Operationen: GET, POST, PUT/PATCH, DELETE).

### ⚙️ Datenbankeinrichtung (XAMPP)

> **Voraussetzung:** [XAMPP](https://www.apachefriends.org/) muss installiert sein.

**Schritt-für-Schritt:**

1. **XAMPP starten**

2. **Backend-Ordner kopieren**
   Im Unity-Projekt befindet sich der Ordner `DBStuff(NotUnity)/MKON` – diesen Ordner in das `htdocs`-Verzeichnis von XAMPP kopieren:
   ```
   C:/xampp/htdocs/MKON
   ```

3. **Datenbank anlegen**
   Im XAMPP Control Panel auf **Admin** neben MySQL klicken (öffnet phpMyAdmin), dann eine neue Datenbank mit dem Namen erstellen:
   ```
   db_mkon
   ```

4. **SQL-Datei importieren**
   In phpMyAdmin die Datenbank `db_mkon` auswählen → Reiter **Importieren** → die `.sql`-Datei aus dem `MKON`-Ordner auswählen → **Ausführen**

5. **Dienste starten**
   Im XAMPP Control Panel vor dem Spielstart sowohl **Apache** als auch **MySQL** starten.

> ⚠️ Apache und MySQL müssen laufen, solange das Spiel aktiv ist, da sonst keine Statistiken gespeichert werden können.

---

## 🛠️ Technische Anforderungen

- **Auflösung:** HD (1920×1080 px)
- **Performance:** mind. 60 FPS auf aktuellen Schullaptops
- **Betriebssystem:** Windows (aktuell)
- **Versionierung:** Git / GitHub
- **Einstiegs-Scene:** `MainMenu`

---

## 🚀 Installation & Start

1. Repository klonen:
   ```bash
   git clone https://github.com/[username]/mario-kart-ohne-nintendo.git
   ```
2. Ordner `Anwendung/` öffnen
3. `.exe` starten – und Gas geben! 🏁

> **Für Entwickler:** Das Unity-Projekt liegt unter `Arbeitsdateien/`. Empfohlene Unity-Version: **6000.3.5f2**

---

## 📁 Abgabe-Struktur

```
GruppenNr_NachnamenDerGruppenMitglieder.zip
├── Konzeption/               # GDD als PDF
├── Arbeitsdateien/
│   └── GME_VornameNachname/  # Quelldateien je Teammitglied
├── Anwendung/                # Lauffähiger Build + ReadMe.txt
├── Trailer/                  # Game Trailer (MP4, 1920×1080)
└── Projektplan.pdf
```

---

## 🎵 Credits & Assets

**3D-Modelle:**
- [Kenney Car Kit](https://kenney.nl/assets/car-kit)
- [Kenney Racing Kit](https://kenney.nl/assets/racing-kit)
- [Kenney Food Kit](https://kenney.nl/assets/food-kit)
- Unity Asset Store: Low Poly Atmospheric Locations, Dungeon Modular Pack, Polylised Medieval Desert City, u. a.

**Audio:**
- [Kart Racer Music Pack](https://assetstore.unity.com/packages/audio/music/electronic/kart-racer-music-pack-353330) (Unity Asset Store)
- [Freesound Community](https://pixabay.com) via Pixabay

*Alle verwendeten Assets sind lizenzfrei oder entsprechend lizenziert.*

---

## 📅 Meilensteine

| Datum | Meilenstein | Status |
|---|---|---|
| 21.01.2026 | Teambildung & GDD-Start | ✅ Erledigt |
| 27.01.2026 | GDD Zwischenabgabe 1 | ✅ Erledigt |
| 04.02.2026 | Fortschrittspräsentation 1 | ✅ Erledigt |
| 11.02.2026 | GDD Zwischenabgabe 2 + Making-of Material | ✅ Erledigt |
| 17.02.2026 | Debug & Polish Phase | 🔄 In Arbeit |
| 18.02.2026 | Fortschrittspräsentation 2 | ✅ Erledigt |
| **27.02.2026** | **Finale Abgabe & Präsentation** | ⏳ Deadline |

---

*Entwickelt im Rahmen des Praxisprojekts – SRH-Fachschule Heidelberg, Fachbereich IT und Medien, PIP-GME24*
