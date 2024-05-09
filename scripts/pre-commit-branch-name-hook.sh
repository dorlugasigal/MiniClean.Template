#!/usr/bin/env bash
LC_ALL=C

local_branch="$(git rev-parse --abbrev-ref HEAD)"

valid_branch_regex="^[A-Za-z0-9_-]+\/(feature|fix)\/(US|us)?-?[0-9]+\/[A-Za-z0-9_-]+$"

message="Branch name $local_branch does not comply with the standard: $valid_branch_regex. Please fix."

if [[ ! $local_branch =~ $valid_branch_regex && ! $local_branch == "main" ]];  
then
    echo "$message"
    exit 1
fi

exit 0
