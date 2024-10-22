#!/bin/bash

set -e

# Файл nuspec
NUSPEC_FILE="PdfForge.nuspec"

# Создадим строку с файлами для добавления, добавив 4 пробела перед каждой строкой
FILES_TO_ADD=""
for so_file in runtimes/linux-x64/native/*.so*; do
    FILES_TO_ADD+="        <file src=\"$so_file\" target=\"runtimes/linux-x64/native/\"/>\n"
done

# Используем awk для вставки перед </files>, сохраняя форматирование
awk -v new_files="$FILES_TO_ADD" '
/<\/files>/ {
    print new_files
}
{ print }
' "$NUSPEC_FILE" > temp.nuspec && mv temp.nuspec "$NUSPEC_FILE"

echo "Библиотеки успешно добавлены в $NUSPEC_FILE"
