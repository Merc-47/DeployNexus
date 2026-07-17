# Deploy Nexus - Git Workflow


## 1. Introduction

This document defines the Git workflow used for Deploy Nexus development.


# 2. Branch Structure


Main branches:


main

Production-ready code


develop

Active development branch


Feature branches:

feature/{feature-name}


Example:

feature/user-authentication


# 3. Development Flow


Requirement

?

Create Feature Branch

?

Development

?

Commit Changes

?

Push Branch

?

Create Pull Request

?

Code Review

?

Merge


# 4. Branch Rules


## main

Rules:

- Production code only
- No direct commits


## develop

Rules:

- Integration branch
- Features are merged here


## Feature Branches

Created from:

develop


Example:


develop

?

feature/create-user


# 5. Commit Standards


Commit messages should describe changes.


Good:


Add user authentication service



Bad:


Changes
Fix
Update



# 6. Pull Request Rules


Every pull request should include:


- Description of changes
- Related issue/story
- Testing information


# 7. Merge Strategy


Preferred:

Feature Branch

?

Pull Request

?

Review

?

Merge into develop


Release:

develop

?

Release Branch

?

main


# 8. Version Tags


Production releases should use tags.


Example:


v1.0.0

v1.1.0

v1.1.1
