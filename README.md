# Individual-Project
An open-world, space-themed survival game where the player's aims are to survive and to find a way back home. Challenges awaits the players as they explore and progress the world where the player must finds ways to overcome them.

This project was created as my final year Undergraduate Dissertation and was built with Unity 2021.3.8f1 with the High-definition Pipeline (HDRP). All assets were created by me.

## Installation

To install this project, either clone this repo and store in your main Unity directory or download the ZIP then add it through Unity Hub, selecting the 'Add project from disk' option and running the project with Unity version 2021.3.8f1.

## Features

The game includes many different features that all work together building up a game.

### Resource harvesting

The player can harvest different resources that are scattered around the map. Different resources may require different means of harvesting such as tool types or harvesting tiers, i.e. the level of the tools thats being used vs the level required to harvest given resouce. These tools can be found or crafted by the player.

### Crafting 

The player can use resources to craft various items and components for many different usages. The player can craft food, items, components, weapons, upgrades and miscellaneous objects. These individual item types are all crafted in their own respective crafting stations, for example, weapons can only be crafted in a weapon crafting station. 

### Building Structures 

Resources can also be used to build structres. The building system is modular and comprises of a grid system where builable objects snap to this grid. When building walls and floors, these structures snap to each other to create a sealed building. There are also furniture that snaps to a smaller grid for a more customizable home.

### Combat and Enemies

Combat consists of melee weapons and ranged weapons where these ranged weapons can shoot rays, instantly hitting the target or projectiles which travels until it makes contact. The player and enemies can utilize these weapons to fight each other. The enemy's behaviour follows a finite state machine model where they may idle and roam around, target and chase the player, stop and attack the player and die.

### Vitals Management

The player must constantly keep their vitals in check where they must manage their health, hydration, hunger and oxygen levels. Health, hydration and hunger can be restored through consuming food and consumables while oxygen can be restored by being in safe spaces such as oxygen bubbles or sealed buildings.

### Saving and Loading

The game includes a saving system to keep track of game progress. This will save all buildings placed by the player including storages and content within them, resources collected, and the player's position and vitals. 
