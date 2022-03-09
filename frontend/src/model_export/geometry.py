class Geometry:

    def __init__(self, tags: list[str] = []) -> None:
        self.__tags = tags

    def has_tag(self, tag: str) -> bool:
        return tag in self.__tags

    def append_tag(self, tag: str) -> None:
        self.__tags.append(tag)

    def get_tags(self) -> list[str]:
        return self.__tags