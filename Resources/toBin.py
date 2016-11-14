from PIL import Image

def main(name):
    im = Image.open(name+".png")
    pix = im.load()
    w,h = im.size
    vals = set()
    matrix = []
    for x in range(w):
        col = []
        for y in range(h):
            value = 0 if pix[x,y] != (0,0,0,0) else 1
            print(value,end='',sep='')
            col.append(value)
            vals.add(pix[x,y])
        matrix.append(col)
        print(vals)
    

if __name__=='__main__':
    main("joytic_cut-photo.ru")
