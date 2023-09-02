# open-social-distributor

**UNDER DEVELOPMENT**

This is a tool for managing social posts across multiple platforms. Simple to configure.

## Documentation

* [Open Social Distributor](https://front-line-tech.com/open-social-distributor/) ([source](docs/index.md))

## TODOs

- [x] Self-test during startup
- [x] Social network configuration (connection strings)
- [ ] Social media features...

| Network | Posting | Threading | Link | Tags | Images |
|-|-|-|-|-|-|
| Console | ✅ | ✅ | ✅ | ✅ | ✅ |
| Mastodon | ✅ | ✅ | ✅ | ✅ | |
| Facebook | ✅ | ✅ | ✅ | ✅ | |
| Discord | | | | | |
| Twitter | | | | | |
| LinkedIn | | | | | |

- [x] Message formatting
    - [x] Support for length limited formatting of threads
    - [x] Support for message breaks: `$$`
    - [x] Force tags into the first message

- [x] CLI features
    - [x] More complex posting options
    - [x] Support for image upload
    - [x] Script publication of CLI binaries
    
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
