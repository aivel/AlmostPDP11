from PIL import Image

def main(name):
    im = Image.open(name+".png")
    pix = im.load()
    w,h = im.size
    vals = set()
    matrix = []
    for x in range(h):
        col = []
        for y in range(w):
            value = ['0' if v<128 else '1' for i,v in enumerate(pix[y,x]) if i<3]
            digit = int('0b'+''.join(value),2)
            print(digit,end='',sep=' ')
            col.append(digit)
            vals.add(digit)
        matrix.append(col)
        print()
   
if __name__=='__main__':
    main("joytic_cut-photo.ru")
