## Profanity Checker
The project allows to check a file for a profanity by a dictionary 
stored in a local database.
There is [Aho-Corasick](https://en.wikipedia.org/wiki/Aho–Corasick_algorithm) 
algorithm for searching patterns in a text

###### Should to know:
* Search is case insensitive
* Punctuation and whitespaces are part of the text as well
* Searching pattern can be a part of a found phrase

> Project is used for a testing purpose only

### How to run
>1. docker-compose up -d
>2. navigate to *http://localhost:9001/swagger/index.html*

### How to use

1. Add words or phrases to the dictionary
   * one by one
   * provide a **file** with line-separated phrases
2. Send a file to the profanity service **(up to 30MB)**

###### Output example:
```json
{
    "hasProfanity": true,
    "profanityItems": [
        {
            "data": "a b",
            "indexes": [
                10323,
                28380,
                48512,
                2463296,
                2463343
            ],
            "fullBounds": [
                "a book",
                "Albania began",
                "a brown",
                "BADIA Batalla",
                "BPA Building,"
            ]
        },
        {
            "data": "ccc",
            "indexes": [
                38142,
                98556,
                2263557,
                2297830,
                2326507
            ],
            "fullBounds": [
                "CCC,",
                "CCC",
                "(CCC)"
            ]
        },
        {
            "data": "hello",
            "indexes": [
                1806014,
                1806096
            ],
            "fullBounds": [
                "Seychellois"
            ]
        }
    ]
}
```
Parameter | Description
------------ | -------------
hasProfanity | Flag that indicates if any profanity found
data | Pattern in the dictionary which was found
indexes | All indexes where pattern was found in a text
fullBounds | The found word (or item) that contains the searching pattern
> **indexes** and **fullBounds** parameters can contain the different number
> of elements (because a **HashSet** under the hood)

### Possible improvements
######   1. Performance and Resilience
- [ ] Optimize [searching algorithm](https://en.wikipedia.org/wiki/String-searching_algorithm)
    * How many languages will you support?
    * How big dictionary will you have?
    * What size of a text or file will be?
- [ ] Split text into chunks
- [ ] Use caching for requests and a dictionary
- [ ] Message bus for a scaling support
- [ ] API versioning
- [ ] Provide an additional incoming adapter with gRPC streaming

###### 2. Healthchecks and Performance metrics
- [ ] Implement [healthchecks mechanism](https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/health-checks?view=aspnetcore-5.0)
- [ ] Add app metrics *(e.g. Prometheus + Grafana)*
- [ ] Check performance by profilers *(e.g. dotTrace)*

###### 3. Security
- [ ] Add validators for REST endpoints *(e.g. FluentValidation)*
- [ ] Provide Authorization mechanism for the API
- [ ] Use TLS encryption
- [ ] Implement rate limiting mechanism 
    
>E.g.: [Rabin–Karp algorithm](https://en.wikipedia.org/wiki/Rabin–Karp_algorithm) uses hashing,
> which reduces the speed on average, especially with the big dictionary, because collision artifacts.

### Technologies and Tools
* .NET 5 + SQLite
* NUnit + Moq + FluentAssertions
* EntityFramework Core
* Swagger
* Serilog
