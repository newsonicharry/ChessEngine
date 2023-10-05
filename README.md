# ChessEngine

--CURRENTLY UNDER MAJOR REDESIGNING--
Reprogrammed the ui due to its messyness
now heavily utilizing structs for moves, and using piecelists
various speed improvement within move generation
move genration has been reprogrammed
pinned pieces function has been optimized heavily
better organization of code
Various other improvements

Expecting a release sometime in mid-late october




Simple chess engine made in C#

Version downloads for x86 Windows:
https://drive.google.com/drive/folders/1mvW5xdzHiLY7cX9XJNeHC_Brk6v9Y116

This is very much a work in progress

Utilizes bitboards and movement masks for move creation
Uses raylib for gui

Todo:
- Fix all bugs with move validation (never ending bugs, dont expect me to get rid of this soon)
- Check for checkmate, 50 move rule, stalemate and repition (because I somehow havent finished that yet)

Engine:
- Make a custom transposition table
- Finish move ordering
- Add endgames
- Utilize multiprocessing
+ And Much More
