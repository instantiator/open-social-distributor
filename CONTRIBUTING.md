# Contributing to Open Social Distributor

First off, thanks for taking the time to contribute! ❤️

All types of contributions are encouraged and valued. See the [table of contents](#table-of-contents) for different ways to help and details about how this project handles them. Please make sure to read the relevant section before making your contribution. It will make it a lot easier for everyone and smooth out the experience for all involved. 

**Looking forward to your contributions! 🎉**

> And if you like the project, but just don't have time to contribute, that's fine. There are other easy ways to support the project and show your appreciation, which we would also be very happy about:
> - Star the project
> - Tweet about it
> - Refer this project in your project's readme
> - Mention the project at local meetups and tell your friends/colleagues

## Table of Contents

- [Code of Conduct](#code-of-conduct)
- [I Have a Question](#i-have-a-question)
- [I Want To Contribute](#i-want-to-contribute)
  - [Reporting Bugs](#reporting-bugs)
  - [Suggesting Enhancements](#suggesting-enhancements)
  - [Your First Code Contribution](#your-first-code-contribution)
  - [Improving The Documentation](#improving-the-documentation)
- [Join The Project Team](#join-the-project-team)

## Code of Conduct

This project and everyone participating in it is governed by the
[Open Social Distributor Code of Conduct](https://github.com/instantiator/open-social-distributorblob/master/CODE_OF_CONDUCT.md).
By participating, you are expected to uphold this code. Please report unacceptable behaviour by creating an [issue](https://github.com/instantiator/open-social-distributor/issues/new) or reaching out to me (contact details at [instantiator.dev](https://instantiator.dev)).

## I Have a Question

> If you want to ask a question, please first take a look through the [documentation](https://instantiator.dev/open-social-distributor/).

Before you ask a question, it is also best to search for existing [issues](https://github.com/instantiator/open-social-distributor/issues) that might help you. In case you have found a suitable issue and still need clarification, you can write your question in this issue. It is also advisable to search the internet for answers first.

If you then still feel the need to ask a question and need clarification, please follow these steps:

- Open an [issue](https://github.com/instantiator/open-social-distributor/issues/new).
- Provide as much context as you can.
- Ask your question.

## I Want To Contribute

> ### Legal Notice
> When contributing to this project, you must agree that you have authored 100% of the content, that you have the necessary rights to the content and that the content you contribute may be provided under the project license.

### Reporting Bugs

#### Before Submitting a Bug Report

A good bug report shouldn't leave others needing to chase you up for more information. Therefore, please don't rush: Investigate carefully, collect information and describe the issue in detail in your report. 

Please complete the following steps in advance to help fix any potential bug as fast as possible:

- Make sure that you are using the latest version.
- Determine if your bug is really a bug and not an error on your side eg. using incompatible environment components/versions (Make sure that you have read the [documentation](https://instantiator.dev/open-social-distributor/). If you are looking for support, you might want to check [this section](#i-have-a-question)).
- To see if other users have experienced (and potentially already solved) the same issue you are having, check if there is not already a bug report existing for your bug or error in the [bug tracker](https://github.com/instantiator/open-social-distributorissues?q=label%3Abug).
- Also make sure to search the internet (including Stack Overflow) to see if users outside of the GitHub community have discussed the issue.
- Collect information about the bug:
  - Stack trace (if available)
  - OS, Platform and Version (Windows, Linux, macOS, x86, ARM), if relevant
  - Version of the CLI, if relevant
  - Your CLI input command config (please remove tokens and any other private information you don't wish to share), and the output you receive
  - Can you reliably reproduce the issue?

#### How Do I Submit a Good Bug Report?

> If you are reporting a security related issue, vulnerability or bug that contains sensitive information, please let us know in the issue tracker - but do not include the sensitive information (eg. details of an exploit). If necessary, we can find a way to share that information safely.

GitHub issues track all the bugs and errors. If you run into an issue with the project:

- Open an [Issue](https://github.com/instantiator/open-social-distributor/issues/new). (No need to label the issue yet.)
- Explain the behaviour you would expect and the actual behaviour you see.
- Please provide as much context as possible and describe the *reproduction steps* that someone else can follow to recreate the issue on their own. This can include code.
  - If it's within your capability, please try to isolate the problem and create a reduced test case.
- Provide the information you collected in the previous section.

Once it's filed:

- The project team will label the issue accordingly.
- A team member will try to reproduce the issue with your provided steps. If there are no reproduction steps or no obvious way to reproduce the issue, the team will ask you for those steps and mark the issue as `needs-repro`. Bugs with the `needs-repro` tag will not be addressed until they are reproduced.
- If the team is able to reproduce the issue, it will be marked `needs-fix`, as well as possibly other tags (such as `critical`), and the issue will be left to be [implemented by someone](#your-first-code-contribution).

### Suggesting Enhancements

This section guides you through submitting an enhancement suggestion for Open Social Distributor, **including completely new features and minor improvements to existing functionality**. Following these guidelines will help maintainers and the community to understand your suggestion and find related suggestions.

#### Before Submitting an Enhancement

- Make sure that you are using the latest version.
- Read the [documentation](https://instantiator.dev/open-social-distributor/) carefully and find out if the functionality is already covered, maybe by an individual configuration.
- Perform a [search](https://github.com/instantiator/open-social-distributor/issues) to see if the enhancement has already been suggested. If it has, add a comment to the existing issue instead of opening a new one.
- Find out whether your idea fits with the scope and aims of the project. It's up to you to make a strong case to convince the project's developers of the merits of this feature. Keep in mind that we want features that will be useful to the majority of our users and not just a small subset. If you're just targeting a minority of users, consider writing an add-on/plugin library.

#### How Do I Submit a Good Enhancement Suggestion?

Enhancement suggestions are tracked as [GitHub issues](https://github.com/instantiator/open-social-distributor/issues).

- Use a **clear and descriptive title** for the issue to identify the suggestion.
- Provide a **step-by-step description of the suggested enhancement** in as many details as possible.
- **Describe the current behaviour** and **explain which behaviour you expected to see instead** and why. At this point you can also mention which alternatives do not work for you.
<!-- - You may want to **include screenshots and animated GIFs** which help you demonstrate the steps or point out the part which the suggestion is related to. You can use [this tool](https://www.cockos.com/licecap/) to record GIFs on macOS and Windows, and [this tool](https://github.com/colinkeenan/silentcast) or [this tool](https://github.com/GNOME/byzanz) on Linux. -->
- **Explain why this enhancement would be useful** to Open Social Distributor users. You may also want to point out the other projects that solved it better and which could serve as inspiration.

### Your First Code Contribution

See the [documentation](docs/index.md) for developer notes.

Here's a simple process for contributing code:

* Fork this repository
* If you're working on more than one contribution, create a branch to work on - otherwise, `main` will do
* Modify the code
* Add unit tests wherever possible
* Commit and push the changes to your repository
* Create a pull request from your repository to here

**NB.** It's not always straightforward to test integrations between the Open Source Distributor and social networks. At the moment, we do not have a shared collection of test accounts. If your code relates to a social network interaction, please provide enough information in your pull request description to test it.

If you're addressing an issue make sure it's clear which issue, in the pull request description.

### Improving The Documentation

Documentation is kept in the `docs/` folder. If you see an error or omission in the docs, feel free to fork, modify, and create a pull request with a fix. (No need to stand on ceremony and create an issue first. Documentation PRs should be reasonably clear about what they're fixing.)

## Join The Project Team

Feel free to reach out to the project team:

* [instantiator](https://github.com/instantiator): [contacts](https://instantiator.dev)

## Attribution

This guide is based on the **contributing-gen**. [Make your own](https://github.com/bttger/contributing-gen)!
