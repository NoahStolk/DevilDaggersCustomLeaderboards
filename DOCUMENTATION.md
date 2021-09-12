# Memory Offsets

## Main

| Data name                  | Offset | Data type                | Size in bytes |
|----------------------------|--------|--------------------------|---------------|
| Marker ("`__ddstats__\0`") |      0 | String                   |            12 |
| Format version             |     12 | 32-bit integer           |             4 |
| Player ID                  |     16 | 32-bit integer           |             4 |
| Player name                |     20 | String                   |            32 |
| Timer time                 |     52 | 32-bit floating point    |             4 |
| Gems collected             |     56 | 32-bit integer           |             4 |
| Enemies killed             |     60 | 32-bit integer           |             4 |
| Daggers fired              |     64 | 32-bit integer           |             4 |
| Daggers hit                |     68 | 32-bit integer           |             4 |
| Enemies alive              |     72 | 32-bit integer           |             4 |
| Level gems                 |     76 | 32-bit integer           |             4 |
| Homing daggers             |     80 | 32-bit integer           |             4 |
| Gems despawned             |     84 | 32-bit integer           |             4 |
| Gems eaten                 |     88 | 32-bit integer           |             4 |
| Gems total                 |     92 | 32-bit integer           |             4 |
| Homing daggers eaten       |     96 | 32-bit integer           |             4 |
| Skull Is alive             |    100 | 16-bit integer           |             2 |
| Skull IIs alive            |    102 | 16-bit integer           |             2 |
| Skull IIIs alive           |    104 | 16-bit integer           |             2 |
| Spiderlings alive          |    106 | 16-bit integer           |             2 |
| Skull IVs alive            |    108 | 16-bit integer           |             2 |
| Squid Is alive             |    110 | 16-bit integer           |             2 |
| Squid IIs alive            |    112 | 16-bit integer           |             2 |
| Squid IIIs alive           |    114 | 16-bit integer           |             2 |
| Centipedes alive           |    116 | 16-bit integer           |             2 |
| Gigapedes alive            |    118 | 16-bit integer           |             2 |
| Spider Is alive            |    120 | 16-bit integer           |             2 |
| Spider IIs alive           |    122 | 16-bit integer           |             2 |
| Leviathans alive           |    124 | 16-bit integer           |             2 |
| Orbs alive                 |    126 | 16-bit integer           |             2 |
| Thorns alive               |    128 | 16-bit integer           |             2 |
| Ghostpedes alive           |    130 | 16-bit integer           |             2 |
| Spider eggs alive          |    132 | 16-bit integer           |             2 |
| Skull Is killed            |    134 | 16-bit integer           |             2 |
| Skull IIs killed           |    136 | 16-bit integer           |             2 |
| Skull IIIs killed          |    138 | 16-bit integer           |             2 |
| Spiderlings killed         |    140 | 16-bit integer           |             2 |
| Skull IVs killed           |    142 | 16-bit integer           |             2 |
| Squid Is killed            |    144 | 16-bit integer           |             2 |
| Squid IIs killed           |    146 | 16-bit integer           |             2 |
| Squid IIIs killed          |    148 | 16-bit integer           |             2 |
| Centipedes killed          |    150 | 16-bit integer           |             2 |
| Gigapedes killed           |    152 | 16-bit integer           |             2 |
| Spider Is killed           |    154 | 16-bit integer           |             2 |
| Spider IIs killed          |    156 | 16-bit integer           |             2 |
| Leviathans killed          |    158 | 16-bit integer           |             2 |
| Orbs killed                |    160 | 16-bit integer           |             2 |
| Thorns killed              |    162 | 16-bit integer           |             2 |
| Ghostpedes killed          |    164 | 16-bit integer           |             2 |
| Spider eggs killed         |    166 | 16-bit integer           |             2 |
| Is player alive            |    168 | 8-bit integer (boolean)  |             1 |
| Is replay                  |    169 | 8-bit integer (boolean)  |             1 |
| Death type                 |    170 | 8-bit integer            |             1 |
| Is in game                 |    171 | 8-bit integer (boolean)  |             1 |
| Replay player ID           |    172 | 32-bit integer           |             4 |
| Replay player name         |    176 | String                   |            32 |
| Survival hash              |    208 | MD5 hash byte array      |            16 |
| Level up time 2            |    224 | 32-bit floating point    |             4 |
| Level up time 3            |    228 | 32-bit floating point    |             4 |
| Level up time 4            |    232 | 32-bit floating point    |             4 |
| Leviathan down time        |    236 | 32-bit floating point    |             4 |
| Orb down time              |    240 | 32-bit floating point    |             4 |
| Game status                |    244 | 32-bit integer           |             4 |
| Max homing                 |    248 | 32-bit integer           |             4 |
| Max homing time            |    252 | 32-bit floating point    |             4 |
| Max enemies alive          |    256 | 32-bit integer           |             4 |
| Max enemies alive time     |    260 | 32-bit floating point    |             4 |
| Max time                   |    264 | 32-bit floating point    |             4 |
| N/A (padding)              |    268 | N/A                      |             4 |
| Graph stats base address   |    272 | 64-bit integer (pointer) |             8 |
| Graph stats count          |    280 | 32-bit integer           |             4 |
| Graph stats loaded         |    284 | 8-bit integer (boolean)  |             1 |
| N/A (padding)              |    285 | N/A                      |             3 |
| Hand level start           |    288 | 32-bit integer           |             4 |
| Homing start               |    292 | 32-bit integer           |             4 |
| Timer start                |    296 | 32-bit floating point    |             4 |
| Prohibited mods            |    300 | 8-bit integer (boolean)  |             1 |

## Graph stats

| Data name                  | Offset | Data type                | Size in bytes |
|----------------------------|--------|--------------------------|---------------|
| Skull Is alive             |      0 | 16-bit integer           |             2 |
| Skull IIs alive            |      2 | 16-bit integer           |             2 |
| Skull IIIs alive           |      4 | 16-bit integer           |             2 |
| Spiderlings alive          |      6 | 16-bit integer           |             2 |
| Skull IVs alive            |      8 | 16-bit integer           |             2 |
| Squid Is alive             |     10 | 16-bit integer           |             2 |
| Squid IIs alive            |     12 | 16-bit integer           |             2 |
| Squid IIIs alive           |     14 | 16-bit integer           |             2 |
| Centipedes alive           |     16 | 16-bit integer           |             2 |
| Gigapedes alive            |     18 | 16-bit integer           |             2 |
| Spider Is alive            |     20 | 16-bit integer           |             2 |
| Spider IIs alive           |     22 | 16-bit integer           |             2 |
| Leviathans alive           |     24 | 16-bit integer           |             2 |
| Orbs alive                 |     26 | 16-bit integer           |             2 |
| Thorns alive               |     28 | 16-bit integer           |             2 |
| Ghostpedes alive           |     30 | 16-bit integer           |             2 |
| Spider eggs alive          |     32 | 16-bit integer           |             2 |
| Skull Is killed            |     34 | 16-bit integer           |             2 |
| Skull IIs killed           |     36 | 16-bit integer           |             2 |
| Skull IIIs killed          |     38 | 16-bit integer           |             2 |
| Spiderlings killed         |     40 | 16-bit integer           |             2 |
| Skull IVs killed           |     42 | 16-bit integer           |             2 |
| Squid Is killed            |     44 | 16-bit integer           |             2 |
| Squid IIs killed           |     46 | 16-bit integer           |             2 |
| Squid IIIs killed          |     48 | 16-bit integer           |             2 |
| Centipedes killed          |     50 | 16-bit integer           |             2 |
| Gigapedes killed           |     52 | 16-bit integer           |             2 |
| Spider Is killed           |     54 | 16-bit integer           |             2 |
| Spider IIs killed          |     56 | 16-bit integer           |             2 |
| Leviathans killed          |     58 | 16-bit integer           |             2 |
| Orbs killed                |     60 | 16-bit integer           |             2 |
| Thorns killed              |     62 | 16-bit integer           |             2 |
| Ghostpedes killed          |     64 | 16-bit integer           |             2 |
| Spider eggs killed         |     66 | 16-bit integer           |             2 |

## Game status

| Meaning                     | Value |
|-----------------------------|-------|
| Title                       |     0 |
| Menu                        |     1 |
| Lobby                       |     2 |
| Playing                     |     3 |
| Dead                        |     4 |
| Own replay from last run    |     5 |
| Own replay from leaderboard |     6 |
| Other player's replay       |     7 |

## Death types

| Death type  | Value |
|-------------|-------|
| FALLEN      |     0 |
| SWARMED     |     1 |
| IMPALED     |     2 |
| GORED       |     3 |
| INFESTED    |     4 |
| OPENED      |     5 |
| PURGED      |     6 |
| DESECRATED  |     7 |
| SACRIFICED  |     8 |
| EVISCERATED |     9 |
| ANNIHILATED |    10 |
| INTOXICATED |    11 |
| ENVENOMATED |    12 |
| INCARNATED  |    13 |
| DISCARNATED |    14 |
| ENTANGLED   |    15 |
| HAUNTED     |    16 |
