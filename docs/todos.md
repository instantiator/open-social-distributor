# TODOs

- [x] Self-test during startup
- [x] Social network configuration (connection strings)

- [ ] Social media features
    - [ ] Posting, formatting, threading, images, links for Mastodon
    - [ ] Posting, formatting, threading, images, links for Facebook
    - [ ] Posting, formatting, threading, images, links for LinkedIn
    - [ ] Posting, formatting, threading, images, links for Discord
    - [ ] Posting, formatting, threading, images, links for Twitter

- [x] Message formatting
    - [x] Support for word wrapping into threads
    - [x] Control where tags appear
    - [x] Control where the link appears
    - [x] Support for message breaks: `$$`

- [x] CLI features
    - [x] More complex posting options
    - [x] Support for image upload
    - [x] Script publication of CLI binaries
    - [x] Allow filtering of networks
    
- [ ] Scheduled posting for service and function
    - [ ] Define message store formats
    - [ ] Document message store formats

- [ ] `DistributionService` (Background Service)
    - [ ] Periodically post from CSV or JSON source (drawn from a URL or file)
    - [ ] Periodically follow more complex post selection rules
    - [x] Docker build and run for the service

- [ ] `DistributionFunction` (Lambda function)
    - [x] AWS CloudWatch stack sync for development
    - [ ] AWS CloudWatch stack publishing
    - [ ] Store state in config
    - [ ] Periodically post from CSV or JSON source (drawn from a URL, S3 bucket, or other source)
    - [ ] Periodically follow more complex post selection rules when invoked

- [ ] Offer Discord icons / updates for the status of other posts
