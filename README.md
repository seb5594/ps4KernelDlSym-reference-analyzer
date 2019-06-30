# ps4sdk ps4KernelDlSym Symbol Resolver

The ps4sdk ps4KernelDlSym Symbol Resolver is a windows forms application (requires .NET 4.0), which has been made to fit my needs for ps4 compiled payloads using [as example my version of Hitodamas open source ps4 sdk](https://github.com/seb5594/ps4sdk) .
This tool has been made to generate dynamically an offsettable containing all kernel symbol names and the corresponeding offsets, where are getting called in a executable. In the current state, it is impossible to resolve stacked function calls.
If i feel to, i will extend this application with the feature of stacked function calls.
You are permitted to use/edit this source code, as long you are referning to this project.

# Requirements (for usage)
- .NET 4.0
- ps4sdk compiled payload(s)
- brain

# How does it work?
This application uses the [open source disassembler library Capstone.NET](https://github.com/9ee1/Capstone.NET), which is a core funcutality to analyze function calls in x86_64 assemblies compiled binarys (payload/s), for further development reasons, by extracting the text sections of the payload elf file using [ElfIO by therifboy](https://github.com/therifboy/ElfIO)
It resolves all absolute calls to ps4KernelDlSym and it outputs a list of all symbol name(s), which are required to execute the payload!
The resulting output will be displayed as a raw list (default) or in a c-styled array (my preferred way).

# Options
The GUI offers the possibility to order the result in execution order or aLphabetical. It is also possible, to output the result in a c-styled array!

# ToDo (coming soon?)
- Resolve stacked symbol names (might be added, when i feel to!)

# Contribute
Feel free to submit pull requests, to improve the development of this project!