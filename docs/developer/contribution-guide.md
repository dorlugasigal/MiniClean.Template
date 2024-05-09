# Contribution Guidelines

Before you start developing, please read the following guidelines.

## Pull Requests

We use pull requests to review and merge code into the main branch.
Please follow the steps below to create a pull request:

1. Create a new branch with the convention of:
   * USERNAME/(FEATURE|FIX)/(WORK-ITEM-ID)/(DESCRIPTION-SEPERATED-BY-DASHES)
2. Make sure the pre-commit hook is installed and working:
    * Install pre-commit using this [link](https://pre-commit.com/#installation)
    * Run `pre-commit run --all-files`
    * [Follow this instructions to install the commands](https://github.com/pocc/pre-commit-hooks?tab=readme-ov-file#information-about-the-commands)
3. Run tests, linters and checks locally and make sure the pipeline is passing
4. Commit your changes to the new branch
5. Make sure the pipeline is passing
6. Make sure you have two reviewers for each PR
7. Once the PR is approved, merge it to the main branch

## Code Style

We use [.NET source code analyzer](https://learn.microsoft.com/en-us/dotnet/fundamentals/code-analysis/overview?tabs=net-8) to enforce code style.
Please make sure you have the pre-commit hook installed and working.

## VS Code

For VS Code it is recommended to install the editorconfig plugin, which will allow the code style
rules defined in .editorconfig (within the root of the project repository) to be applied
during development.
