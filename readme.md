# Algorithmic-Folding

The code base for the Algorithmic Folding class (Winter Term 2020/21). **Each lecture** or set of lectures should have a **separate branch** for its codebase.

## General Advice

1. Never push to `master`
2. For each branch, add a section "*How to Run*" in the `readme.md` that explains, how to run the code
3. Write imperative commit messages, that would finish the sentence "If applied, this commit will `Change the behaviour of class x`."
4. Describe what was done and why, but not how

## How to run

Import this branch into a repl.it C# project. You can import directly with a premium account, or you can download the repository first and then upload the files to the project.

## Folding simulation

To get a clue how the strip will look like after folding (at least for small strips), you can download the output (*export.svg*) and upload it to <https://origamisimulator.org>. The integer *stripWidth* has to be 3 or more for that simulation to work.

## Use other input files

To use other input files, upload a .stl-file and change the string *importFile* in main.cs to the file's name. Please note that only non-binary .stl-files can be imported.

## Debug option

The bool *DEBUG* in main.cs indicates wether debugging output is created or not. Debugging output includes console output as well as an .svg-file (*debug.svg*) which shows how the individual triangles should look like after folding. The integer *stripWidth* should be 1 or less so you can identify the single triangles.
