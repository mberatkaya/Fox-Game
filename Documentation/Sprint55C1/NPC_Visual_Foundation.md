# Sprint 5.5-C.1 NPC Visual Foundation

Status: Manual download/import required before Sprint 5.5-E final NPC placement.

Approved source:
- Quaternius Universal Base Characters
- Official page: https://quaternius.com/packs/universalbasecharacters.html
- License: CC0
- Fit: stylized, humanoid-retargetable, compatible with Quaternius Universal Animation Library

Small import target:
- 1 Regular male base character
- 1 Regular female base character
- 3-5 readable hairstyles
- Shared character material/textures needed by those selected meshes

Approved animation source, only if the base package does not include enough NPC motion:
- Quaternius Universal Animation Library
- Official page: https://quaternius.com/packs/universalanimationlibrary.html
- License: CC0
- Small clip target: neutral idle, relaxed idle, talk/gesture, wave, sit, walk

Do not import:
- Full character library
- Full 120+ animation library
- Combat, death, gun, tactical, or modern/sci-fi clips

Sprint 5.5-E integration guardrails:
- Keep GuideNpc.cs, DialoguePanelUI, quest state, save state, and dialogue architecture unchanged unless Sprint 5.5-E explicitly asks for gameplay integration.
- Use a Humanoid Avatar and verify retargeting in Unity before selecting the final Guide NPC.
- Root motion should remain disabled for any gameplay-driven NPC movement.
