# Algorithmic-Folding
The code base for the Algorithmic Folding class (Winter Term 2020/21). **Each lecture** or set of lectures should have a **separate branch** for its codebase.

## General Advice
1. Never push to `master`
2. For each branch, add a section "*How to Run*" in the `readme.md` that explains, how to run the code
3. Write imperative commit messages, that would finish the sentence "If applied, this commit will ` Change the behaviour of class x`."
4. Describe what was done and why, but not how

## How to Run
Import this branch into a [repl.it](repl.it) Typescript project. You can import directly with a premium account, or you can download the repository first and then upload the files to the project. The "main file" is `index.ts`.

## How to Import Models
Go to [kyub.com](kyub.com) and open a model in the editor. Add `#svgExport.exportModelAsText=true` to the URL and reload the page. Then click `im-/export -> make this` and open the JavaScript console. Wait for the export pipeline to finish (it will stop with an error) and then copy the json string printed into the console. Paste the string into a new document, like `imports/newImport.json` and set the location of the import in `index.ts` to import the model.
