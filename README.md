# Noble Compass

A Mount & Blade II: Bannerlord mod (v1.4.8) that puts a compass arrow around your party on the campaign map, pointing to a noble you choose in the encyclopedia. The compass only appears once your scouts are close enough to pick up the noble's trail, and the better your party's Scouting skill, the farther away that is.

## How it works

- Open a lord's or lady's page in the encyclopedia and click the compass button next to the bookmark star. Click it again to stop. Only one noble is followed at a time; picking another switches to them.
- When the noble is within range, an arrow appears beside your party, circling it to point towards them. It turns with the camera. Optionally the arrow changes color as you get closer (green when near, red at the edge of your range, by default).
- Range = *Range with 0 Scouting* + *Extra range per Scouting point* x your scout's Scouting skill (your own if no scout is assigned). With the defaults that's 20 map distance at 0 Scouting, 50 at 100 and 110 at 300.
- Under your party it says "Looking for" and the noble's name.
- A message tells you when your scouts pick up or lose the trail. If the noble dies, the compass stops.
- The compass points to where the noble really is: their party, the settlement they're in, or the party holding them prisoner.
- The noble you follow is kept in your save. Removing the mod is safe; the save just forgets the choice.

## Options (Mod Options, MCM)

| Option | Default | What it does |
|---|---|---|
| Enable | On | Turns the mod on or off. Off hides the compass and the encyclopedia button. |
| Track any hero, not only nobles | Off | Shows the compass button for every living hero (companions, wanderers, notables), not only lords and ladies. |
| Only heroes you have met | Off | You can only follow someone you have met in person; for anyone else the button just says you haven't met them yet. |
| Range with 0 Scouting | 20 | Map distance at which the compass appears when your scout has no Scouting. |
| Extra range per Scouting point | 0.30 | Range added for each point of Scouting. |
| Unlimited range | Off | Shows the compass at any distance. |
| Compass size | 150 | Width of the circle the arrow travels around your party, in UI units. |
| Show who you're looking for | On | The "Looking for ..." label under your party. |
| Arrow color | #F2C97A | Arrow color (#RRGGBB) when not colored by distance. |
| Color the arrow by distance | Off | Blends the arrow from the far color to the near color as you close in. |
| Near color | #5FE05F | Arrow color right next to the noble. |
| Far color | #E04A3A | Arrow color at the edge of your scouting range. |
| Announce when the trail is found or lost | On | Message at the top of the screen when the compass appears or disappears. |
| Debug messages | Off | Every few seconds, shows the distance, Scouting skill and range in the chat log, for tuning. |

## Requirements

Bannerlord.Harmony, ButterLib, UIExtenderEx and Mod Configuration Menu v5. Load it after them.

## Building

From the `src` folder run `dotnet build -c Release`. The build copies the dll, `SubModule.xml`, `ModuleData` and `GUI` into `Modules\NobleCompass` in the game folder. If the game is installed somewhere else, pass `/p:GameFolder="..."`.

## Translations

All English text is in the code as `{=id}English text`. `Module/ModuleData/Languages/std_NobleCompass.xml` lists every string and is the template for translations.

To add a language, for example German, run `python BannerlordModTools/bltools.py loc add-language NobleCompass DE` from the `repos` folder. It creates `Module/ModuleData/Languages/DE/` with `language_data.xml` and a translation file where every line is marked NEW; translate each `text` attribute, keeping the ids and every `{VARIABLE}` unchanged. Running `loc update` after the code changes marks new, changed and obsolete lines.

## Support

If you enjoy the mod, you can support me on [Ko-fi](https://ko-fi.com/darkjeffrey).
