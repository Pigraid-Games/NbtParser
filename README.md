# NbtParser

## Overview
NbtParser: A utility library for parsing NBT (Named Binary Tag) data into JSON and vice versa, facilitating seamless dta exchange between MiNET Minecraft Server objects and external systems.

## Features
* Convert NBT structures (NbtCompound) to JSON for easy manipulation.
* Parse JSON back into NBT structures for MiNET Minecraft Server operations.
* Supports Vvarious NBT tag types, including:
  * Primivite types: `Byte`, `Short`, `Int`, `Long`, `Float`, `Double`
  * Complex types: `String`, `ByteArray`, `IntArray`, `LongArray`, `Compound`, `List`
* Handles nested NBT structures.

## Installation
### Prerequisites
* A Minecraft server using MiNET.
* If you are using a fork of MiNET, make sure that it uses the nugget package `MiNET.fnbt v1.0.22`

### Setup
1. Clone the repository
```bash
git clone https://github.com/Pigraid-Games/NbtParser.git
```
2. Build the project
```bash
dotnet build
```

## Usage
### NBT To Json
```c#
var inventory = player.Inventory;
// Note that if ExtraData is null, NbtToJson returns null
var serializedData = NbtParser.ParseToJson(inventory.Helmet.ExtraData);
```
### Json To NBT
```c#
var nbtCompound = NbtParser.ParseFromJson(serializedData);
if(nbtCompound != null)
{
    inventory.Helmet.ExtraData = nbtCompound;
}
```
