# Stardew Valley Assistant Mod

## Overview
The Stardew Valley Assistant is a mod designed to enhance gameplay by providing intelligent, real-time recommendations for optimizing in-game decisions. The assistant helps players manage time, money, and resources more effectively, improving their overall experience.

## Features
- **Crop Selection:** Suggests the best crops to plant based on season, day, available funds, and profitability.
- **Budget Management:** Advises on spending priorities for seeds and upgrades.
- **Item Usage:** Provides information on crafting, gifting, and selling various items.
- **Relationship Building:** Offers insights into character preferences and schedules to help improve relationships.
- **Processing Advice:** Recommends the most profitable ways to refine and process materials.
- **Resource Gathering:** Suggests efficient methods for collecting materials like wood, stone, ores, and forageables.

## Technical Details
- **Modding Framework:** Implemented using the SMAPI framework in C#.
- **Data Storage:** Using open source [Marqo](https://www.marqo.ai/)
- **Natural Language Processing:** Integrates an LLM API to handle player queries and generate responses.

## Project Timeline
- **Jan 30:** Finalize project requirements, gather game data, research libraries, and set up the development environment.
- **Feb 13:** Implement basic mod functionality, including user input handling and game event storage.
- **Feb 27:** Integrate database, develop search algorithm, and experiment with LLM API connections.
- **Mar 20:** Implement advanced query handling and tree search algorithms.
- **Apr 3:** Conduct testing and final optimizations.

## Validation Plan
- **Functional Testing:** Verify core features such as crop selection and budget management.
- **Interface Testing:** Ensure smooth communication between user input and mod output.
- **Usability Testing:** Gather feedback from players to enhance user experience and usability.

## Installation
1. Install [SMAPI](https://smapi.io/) (Stardew Valley modding API).
2. Download the Stardew Valley Assistant mod files.
3. Place the mod folder into the `Mods` directory within your Stardew Valley game folder.
4. Launch the game through SMAPI to activate the mod.

## Future Enhancements
- Improved AI-driven recommendations
- Expanded support for more game mechanics
- Additional language support for broader accessibility

---
Note: This is not the final README, as some aspects of the project may change throughout development.
