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
dotnet run --project=Generator/Generator.csproj --configuration=Release -- --file-size 1gb --file-name test.txt
```

`--file-size` can be mb or gb. There is no default value so it must be passed. I would not recommend using 10+gb as it might take some time.

`--file-name` is the name of the file that will be generated. There is not default value. Just use `test.txt` or something.

`--help` or `-h` if you wish to see all available options.

### Performance

On M2 MacBook Air it takes ~30 seconds to generate a 1gb file.

![generator_perf](generator_screenshot.png 'generator_perf')

### How it works

It generates n number of files in temp directory of the system. The "n" is the number of processors available on the machine, this is important because it generates those files in parallel. After it is done it merges the file into one bigger, final file.