import numpy as np

existing_links = [[('Link_X8E_2', 'Link_2YE_53', 'Link_Q02_35')], [('Link_X8E_2', 'Link_RM3_48', 'Link_Z3V_49')], [('Link_X8E_2', 'Link_2fE_53', 'Link_Q92_35')]]

if __name__ == '__main__':
    shared_links = []
    lists = list()
    for element in existing_links:
        for entry in element:
            entry_list = list(entry)
            lists.append(set(entry_list))
    # return set.intersection(*lists)
