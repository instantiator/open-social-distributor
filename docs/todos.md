# TODOs

- [x] Self-test during startup
- [x] Social network configuration (connection strings)

- [ ] Social media features
    - [ ] Image support for Mastodon
    - [ ] Image support for Facebook
    - [ ] Posting, formatting, images, links for Discord
    - [ ] Posting, formatting, images, links for Twitter
    - [ ] Posting, formatting, images, links for LinkedIn

- [x] Message formatting
    - [x] Support for word wrapping into threads
    - [x] Control where tags appear
    - [x] Control where the link appears
    - [x] Support for message breaks: `$$`

- [x] CLI features
    - [x] More complex posting options
    - [x] Support for image upload
    - [x] Script publication of CLI binaries
    - [ ] Allow filtering of networks to post to
    
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
