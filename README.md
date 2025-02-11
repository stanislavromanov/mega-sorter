# Generator

Generator is responsible for generating the test file that follows the format as `{number}. {company_name}`, here is an example

```text
32233. Google
7644. Apple
2. Microsoft
99. Apple
8547854. Netflix
```

### How to run

To run the generator, you can use the following command
```bash
dotnet restore
time dotnet run --project=Generator/Generator.csproj --configuration=Release -- --file-size 1gb --file-name test.txt
```

`--file-size` can be mb or gb. There is no default value so it must be passed. I would not recommend using 10+gb as it might take some time.

`--file-name` is the name of the file that will be generated. There is not default value. Just use `test.txt` or something.

`--help` or `-h` if you wish to see all available options.

### Performance

On M2 MacBook Air it takes ~10 seconds to generate a 1gb file.

![generator_perf](generator_screenshot.png 'generator_perf')

### How it works

It generates n number of files in temp directory of the system. The "n" is the number of processors available on the machine, this is important because it generates those files in parallel. After it is done it merges the file into one bigger, final file.

# Sorter

Sorter is responsible for taking the random generated file and sort it. First alphabetically and then by the number assigned in front. It will save the file as a new one with `_sorted` affix.

### How to run

```bash
dotnet restore
time dotnet run --project=Sorter/Sorter.csproj --configuration=Release -- --file-name test.txt
```

`--file-name` is the name of the file that will be sorted. There is not default value. Just use `test.txt` or whatever you used to generate the data.

`--help` or `-h` if you wish to see all available options.

### Performance

On M2 MacBook Air it takes ~5 minutes to sort a 1gb file.
![sorting_perf](sorting_perf.png 'sorting_perf')

### How it works
1. It splits the file into chunks of 128mb each.
2. It sorts the chunks in parallel.
3. It merges the chunks into one big file using the merge-sort plucking method of taking first line of every chunk and plucking the samlles piece, then replacing the index of that chunk with the next line from same chunk.

# Notes

1. There are no memory leaks, however when using the debugger on sorting a lot of memory goes to gen2 which is not good. Not sure if it is related to debugging itself tho and would like to avoid using GC.Collect() manually. ![sorting_mem](sorting_mem.png 'sorting_mem')
2. The performance obviously can be improved by using binary data instead of text for reasons like less CPU usage and consistent memory usage per row. I would have to consult ChatGPT for that however since I do not know how to do that.
