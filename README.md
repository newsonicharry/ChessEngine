# ChessEngine
Simple chess engine made in C#

Utilizes bitboards and movement masks for move creation
Uses raylib for gui
Version downloads for x86 Windows:
https://drive.google.com/drive/folders/1mvW5xdzHiLY7cX9XJNeHC_Brk6v9Y116

--CURRENTLY UNDER MAJOR REDESIGNING--
Reprogrammed the ui due to its messyness
now heavily utilizing structs for moves, and using piecelists
various speed improvement within move generation
move genration has been reprogrammed
pinned pieces function has been optimized heavily
better organization of code
Various other improvements

Update: Testing is showing around a 5x performance increase,
with better branch pruning, far higher depths should be able to be acheived.

The engine is functioning right now, but suffers from occasional errors when running for too long or at too high of a depth.
Also occasionaly plays seemingly terrible moves.
Still hoping for a release in late october, but unexpected issues may arise. 


