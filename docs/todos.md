# TODOs

- [x] Self-test during startup
- [x] Social network configuration (connection strings)

- [x] Social media features
    - [x] Posting, formatting, threading, images, links for Mastodon
    - [x] Posting, formatting, threading, images, links for Discord
    - [x] Posting, formatting, threading, images, links for Facebook
    - [x] Posting, formatting, threading, images, links for LinkedIn

- [x] Message formatting
    - [x] Support for word wrapping into threads
    - [x] Control where tags appear
    - [x] Control where the link appears
    - [x] Support for message breaks: `$$`
    - [x] Per-network image assignment choices

- [x] Unit testing (setup)
    - [x] Full suite of test projects
    - [x] Library tests of a good junk of functionality

- [x] Integration testing (setup)
    - [x] Simple integration test suite
    - [x] Mastodon test account
    - [x] Simple Mastodon integration tests
    - [x] Simple Discord integration tests

- [x] CLI features
    - [x] More complex posting options
    - [x] Support for image upload
    - [x] Script publication of CLI binaries
    - [x] Allow filtering of networks
    - [x] Allow creation of messages from JSON format (parameter or from a message store (optionally indexed))
    - [ ] Allow directly specified json
    
- [x] Scheduled posting for service and function
    - [x] Define message store formats
    - [x] Document message store formats

- [ ] `DistributionService` (Background Service)
    - [x] Docker build and run for the service
    - [ ] Periodically post from CSV or JSON source (drawn from a URL or file)
    - [ ] Periodically follow more complex post selection rules

- [ ] `DistributionFunction` (Lambda function)
    - [x] AWS CloudWatch stack sync for development
    - [ ] AWS CloudWatch stack publishing
    - [ ] Store state in config
    - [ ] Periodically post from CSV or JSON source (drawn from a URL, S3 bucket, or other source)
    - [ ] Periodically follow more complex post selection rules when invoked

- [ ] Add Discord emoji / icons / updates for the status of other posts
