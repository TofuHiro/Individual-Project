# Individual-Project - Lost Horizons

![Cover Image](https://github.com/TofuHiro/Individual-Project/blob/main/media/LH_Cover.png)

An open-world, space-themed survival game where the player's aims are to survive and to find a way back home. Challenges awaits the players as they explore and progress the world where the player must finds ways to overcome them.

This project was created as my final year Undergraduate Dissertation and was built with Unity 2021.3.8f1 with the High-definition Pipeline (HDRP). The main goal for this project was to showcase complexity, what I learnt through out my years of studying and my passion for creating games. Most assets were created by me; the majority of the models were created with Blender, the animations created internally with Unity and some other assets such as sound effects and a few models are creditted in the full report. 

Full technical documentation available at docs/Individual Project - Final Report.pdf.

## Installation

To install this project, either clone this repo and store in your main Unity directory or download the ZIP then add it through Unity Hub, selecting the 'Add project from disk' option and running the project with Unity version 2021.3.8f1.

## Game Features

### Resource harvesting

![Gameplay GIF](https://github.com/TofuHiro/Individual-Project/blob/main/media/Harvest.gif)

The player can harvest different resources that are scattered around the map. Different resources may require different means of harvesting such as tool types or harvesting tiers, i.e. the level of the tools thats being used vs the level required to harvest given resouce. These tools can be found or crafted by the player.

### Crafting 

![Gameplay GIF](https://github.com/TofuHiro/Individual-Project/blob/main/media/Craft.gif)

The player can use resources to craft various items and components for many different usages. The player can craft food, items, components, weapons, upgrades and miscellaneous objects. These individual item types are all crafted in their own respective crafting stations, for example, weapons can only be crafted in a weapon crafting station. 

### Building Structures 

![Gameplay GIF](https://github.com/TofuHiro/Individual-Project/blob/main/media/Build.gif)

Resources can also be used to build structres. The building system is modular and comprises of a grid system where builable objects snap to this grid. When building walls and floors, these structures snap to each other to create a sealed building. There are also furniture that snaps to a smaller grid for a more customizable home.

### Combat and Enemies

![Gameplay GIF](https://github.com/TofuHiro/Individual-Project/blob/main/media/Combat.gif)

Combat consists of melee weapons and ranged weapons where these ranged weapons can shoot rays, instantly hitting the target or projectiles which travels until it makes contact. The player and enemies can utilize these weapons to fight each other. The enemy's behaviour follows a finite state machine model where they may idle and roam around, target and chase the player, stop and attack the player and die.

### Vitals Management

The player must constantly keep their vitals in check where they must manage their health, hydration, hunger and oxygen levels. Health, hydration and hunger can be restored through consuming food and consumables while oxygen can be restored by being in safe spaces such as oxygen bubbles or sealed buildings.

## Technical Features

### Saving and Loading

![Gameplay GIF](https://github.com/TofuHiro/Individual-Project/blob/main/media/Saving.gif)

The game includes a saving system to keep track of game progress. This will save all buildings placed by the player including storages and content within them, resources collected, and the player's position and vitals. 

### Procedural Generation

![Gameplay GIF](https://github.com/TofuHiro/Individual-Project/blob/main/media/Marching.gif)

Harvestable resource nodes are generated around the map where their shape varies based on a marching cube algorithm used to generate the mesh of the nodes. This saved alot of time when designing the map as well as added alot of variety to the map as each node is likely to be unique.

### Custom Object Pooling

As this game contains a large amount of objects that the player can pick up, utilizing an object pooler pattern was a must. However, implementing it in the standard way could lead to unwanted high memory usage. As there are many different objects, creating a pool for each of these could do more harm than good, so I modified the pattern so that pools are dynamically generated as the player interacts with objects. The trade off of instantiating object during the game compared to filling up the memory with possibly unused objects is what I took.

### Building Structures - Sealing System

Structures built by the players are modular as each wall and floor is used to determine whether a built system is enclosed or not, which allows the possibility to breathe within that structure. This is done and can be done by using 3-dimensional array as the building system is grid like where each grid would represent an index in the 3D array. A depth-first search algorithm is then used to traverse the 3D array to check for unsealed spots/breaches. This get complicated when joining and disconnecting two structures by bridging them together with a floor/wall or deleting a floor/wall that would have connected a separated system.  

### Finite State Machine

A simple finite state machine is used to control the enemy AI in the game. The AI loops through the process of detecting if a player is nearby, chasing them, then attempt to attack them with the weapon they equip.

### World Space UI

![Gameplay GIF](https://github.com/TofuHiro/Individual-Project/blob/main/media/Invent.gif)

Storages make use of world space UI where they display their contents in a grid where the player may drag icons of the items within either inventory across to another slot to move items between storages.
