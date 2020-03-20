# InfectedServer script by S3VDITO
### This script based on IS 1.0 (old version)

This script is made to make changes to the infection mode gameplay.

## What is implemented
* Kill streaks system
* Osprey
* Lunge Zones (On maps: Resistance, Terminal, Underground)
* Anticamp system (If used MOAB or Juggernaut)
* AC130

## Bugs
* Knife kills sometimes break a series of killers and cannot be used later (bomb killings too)
* If you call more than 8 helicopter, the server will shut down (not a bug, just for now I'm too lazy to check for the number of helicopters in air)

## TODO
* Remote turret on PaveLow
* Deat and kill steaks for infected

## Architecture
### Folder KILLSTREAKS
* AC130.cs - Contains AC130 main functions
* CarePackage.cs - Contains the necessary functions for trigger objects (packages or thing bag)
* DamageFeedBack.cs - Display type hit marker
* Juggernaut.cs - Handling kill and damage event for Juggernaut
* Laptop.cs - Not implemented
* Perks.cs - Contains all perk bonus func (detect explosive not working)
* SignalGrenade.cs - Handling signal grenade fire event
* Vehicles.cs - Contains all vehicles for droping crates

### ROOT
* AntiCampClass.cs - Contains anticamp functions
* DebugClass.cs - Contains Debug functions
* GamePlay.cs - Contains main GamePlay
* HUDClass.cs - Contains main huds (Entity extensions)
* InitializateClass.cs - Contains init function (global and local mapping)
* LevelClass.cs - Initializate primary values on map
* MapClass.cs - Contains simple map edit
* MemoryClass.cs - Contains functions for works with game memory
* ModelClass.cs - Contains change model players functions
* SoundClass.cs - Contains function for play sound
* WeaponClass.cs - Contains weapons for humans